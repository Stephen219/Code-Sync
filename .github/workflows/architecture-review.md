
name: Architecture Review

on:
  schedule:
    - cron: '*/30 * * * *'
  workflow_dispatch:

permissions:
  contents: read
  issues: write

jobs:
  review:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4

      - name: Collect codebase context
        run: |
          python3 << 'PYEOF'
          import os, glob

          out = []
          out.append("# CODE-SYNC / FILESYNC ARCHITECTURE REVIEW")
          out.append("")

          # 1. Project structure
          out.append("## 1. PROJECT STRUCTURE")
          for root, dirs, files in os.walk("."):
              dirs[:] = [d for d in dirs if d not in ("obj", "bin", ".vs", ".git", "node_modules")]
              for f in sorted(files):
                  if f.endswith((".cs", ".csproj", ".json", ".md")):
                      out.append(os.path.join(root, f))
          out.append("")

          # 2. Project references
          out.append("## 2. PROJECT REFERENCES")
          for root, dirs, files in os.walk("."):
              dirs[:] = [d for d in dirs if d not in ("obj", "bin", ".vs", ".git")]
              for f in files:
                  if f.endswith(".csproj"):
                      path = os.path.join(root, f)
                      out.append(f"### {path}")
                      with open(path) as fh:
                          for line in fh:
                              if "ProjectReference" in line or "PackageReference" in line:
                                  out.append(line.strip())
                      out.append("")

          # 3. Dependency violations
          out.append("## 3. DEPENDENCY VIOLATION SCAN")
          violations = {
              "FileSync.Domain": ["FileSync.Application", "FileSync.Infrastructure", "FileSync.Api", "FileSync.Web"],
              "FileSync.Application": ["FileSync.Infrastructure", "FileSync.Api", "FileSync.Web"],
          }
          for layer, banned in violations.items():
              out.append(f"### {layer} (should NOT reference: {', '.join(banned)})")
              found = False
              if os.path.isdir(layer):
                  for root, dirs, files in os.walk(layer):
                      dirs[:] = [d for d in dirs if d not in ("obj", "bin")]
                      for f in files:
                          if f.endswith(".cs"):
                              filepath = os.path.join(root, f)
                              with open(filepath, errors="replace") as fh:
                                  for i, line in enumerate(fh, 1):
                                      for b in banned:
                                          if f"using {b}" in line:
                                              out.append(f"  VIOLATION: {filepath}:{i} -> {line.strip()}")
                                              found = True
              if not found:
                  out.append("  CLEAN")
              out.append("")

          # 4. All source files
          out.append("## 4. SOURCE FILES")
          for root, dirs, files in os.walk("."):
              dirs[:] = [d for d in dirs if d not in ("obj", "bin", ".vs", ".git", "node_modules", "Migrations")]
              for f in sorted(files):
                  if f.endswith(".cs"):
                      filepath = os.path.join(root, f)
                      try:
                          with open(filepath, errors="replace") as fh:
                              content = fh.read()
                          lines = content.splitlines()
                          out.append(f"### FILE: {filepath} ({len(lines)} lines)")
                          if len(lines) <= 300:
                              out.append(content)
                          else:
                              out.append("\n".join(lines[:150]))
                              out.append(f"... (truncated - {len(lines)} total) ...")
                              out.append("\n".join(lines[-100:]))
                          out.append("")
                      except Exception as e:
                          out.append(f"### FILE: {filepath} (ERROR: {e})")
                          out.append("")

          # 5. Documentation
          out.append("## 5. DOCUMENTATION")
          for root, dirs, files in os.walk("."):
              dirs[:] = [d for d in dirs if d not in ("obj", "bin", ".vs", ".git", "node_modules")]
              for f in sorted(files):
                  if f.endswith(".md"):
                      filepath = os.path.join(root, f)
                      try:
                          with open(filepath, errors="replace") as fh:
                              out.append(f"### DOC: {filepath}")
                              out.append(fh.read())
                              out.append("")
                      except:
                          pass

          # 6. Config files
          out.append("## 6. CONFIGURATION FILES")
          for root, dirs, files in os.walk("."):
              dirs[:] = [d for d in dirs if d not in ("obj", "bin", ".vs", ".git")]
              for f in sorted(files):
                  if f in ("Program.cs", "Startup.cs") or (f.startswith("appsettings") and f.endswith(".json")):
                      filepath = os.path.join(root, f)
                      try:
                          with open(filepath, errors="replace") as fh:
                              out.append(f"### CONFIG: {filepath}")
                              out.append(fh.read())
                              out.append("")
                      except:
                          pass

          # 7. Metrics
          cs_files = []
          total_lines = 0
          csproj_count = 0
          for root, dirs, files in os.walk("."):
              dirs[:] = [d for d in dirs if d not in ("obj", "bin", ".vs", ".git")]
              for f in files:
                  if f.endswith(".cs"):
                      cs_files.append(os.path.join(root, f))
                      try:
                          with open(os.path.join(root, f), errors="replace") as fh:
                              total_lines += sum(1 for _ in fh)
                      except:
                          pass
                  if f.endswith(".csproj"):
                      csproj_count += 1

          out.append("## 7. METRICS")
          out.append(f"Total .cs files: {len(cs_files)}")
          out.append(f"Total C# lines: {total_lines}")
          out.append(f"Projects: {csproj_count}")

          result = "\n".join(out)
          with open("/tmp/context.txt", "w") as f:
              f.write(result)

          print(f"Collected {len(cs_files)} files, {total_lines} lines, {len(result)} chars")
          PYEOF

      - name: Run Claude review
        env:
          ANTHROPIC_API_KEY: ${{ secrets.ANTHROPIC_API_KEY }}
        run: |
          python3 << 'PYEOF'
          import json, os, urllib.request, urllib.error, sys

          api_key = os.environ.get("ANTHROPIC_API_KEY", "")
          if not api_key:
              print("ERROR: ANTHROPIC_API_KEY secret is not set")
              sys.exit(1)

          with open(".github/prompts/review-prompt.md") as f:
              prompt = f.read()
          with open("/tmp/context.txt") as f:
              context = f.read()

          message = f"{prompt}\n\n{context}"
          print(f"Sending {len(message)} chars to Claude...")

          payload = json.dumps({
              "model": "claude-sonnet-4-20250514",
              "max_tokens": 8096,
              "messages": [{"role": "user", "content": message}]
          }).encode("utf-8")

          req = urllib.request.Request(
              "https://api.anthropic.com/v1/messages",
              data=payload,
              headers={
                  "Content-Type": "application/json",
                  "x-api-key": api_key,
                  "anthropic-version": "2023-06-01",
              },
          )

          try:
              with urllib.request.urlopen(req, timeout=180) as resp:
                  data = json.loads(resp.read().decode())
          except urllib.error.HTTPError as e:
              print(f"HTTP {e.code}: {e.read().decode()}")
              sys.exit(1)
          except Exception as e:
              print(f"Error: {e}")
              sys.exit(1)

          text = "\n".join(b["text"] for b in data.get("content", []) if b.get("type") == "text")
          with open("/tmp/review.json", "w") as f:
              f.write(text)

          print(f"Done: {len(text)} chars, tokens: {data.get('usage', {})}")
          PYEOF

      - name: Create GitHub Issue
        env:
          GITHUB_TOKEN: ${{ secrets.GITHUB_TOKEN }}
        run: |
          python3 << 'PYEOF' > /tmp/issue-body.md
          import json, re

          with open("/tmp/review.json") as f:
              raw = f.read()

          data = None
          for attempt in [
              lambda: json.loads(re.search(r'```json\s*(.*?)\s*```', raw, re.DOTALL).group(1)),
              lambda: json.loads(raw),
              lambda: json.loads(raw[raw.find('{'):raw.rfind('}')+1]),
          ]:
              try:
                  data = attempt()
                  break
              except:
                  continue

          if not data:
              print(f"## Architecture Review\n\n{raw}")
              exit()

          health = data.get('overallHealth', '?').upper()
          icon = {"GREEN":"🟢","YELLOW":"🟡","RED":"🔴"}.get(health, "⚪")
          print(f"## {icon} Architecture Review — {health}\n")
          print(f"{data.get('summary','')}\n")

          if data.get('positives'):
              print("## ✅ Positives\n")
              for p in data['positives']:
                  print(f"- {p}")
              print()

          if data.get('findings'):
              print("## 🔍 Findings\n")
              for f in data['findings']:
                  si = {"critical":"🔴","warning":"🟡","info":"🔵"}.get(f.get("severity",""),"⚪")
                  print(f"### {si} {f.get('id','')} — {f.get('title','')}\n")
                  print(f"**Severity:** {f.get('severity','')} | **Effort:** {f.get('estimatedEffort','')}\n")
                  print(f"{f.get('description','')}\n")
                  for a in f.get('affectedFiles', []):
                      print(f"- `{a}`")
                  if f.get('affectedFiles'): print()
                  print(f"**Suggestion:** {f.get('suggestion','')}\n")
                  if f.get('acceptanceCriteria'):
                      print("**Acceptance criteria:**")
                      for c in f['acceptanceCriteria']:
                          print(f"- [ ] {c}")
                      print()
                  if f.get('definitionOfDone'):
                      print("**Definition of done:**")
                      for d in f['definitionOfDone']:
                          print(f"- [ ] {d}")
                      print()
                  print("---\n")

          if data.get('userStories'):
              print("## 📋 User Stories\n")
              for s in data['userStories']:
                  print(f"### {s.get('title','')}\n")
                  print(f"{s.get('description','')}\n")
                  print(f"**Points:** {s.get('storyPoints','?')} | **Priority:** {s.get('priority','?')}\n")
                  if s.get('acceptanceCriteria'):
                      print("**Acceptance criteria:**")
                      for c in s['acceptanceCriteria']:
                          print(f"- [ ] {c}")
                      print()
                  if s.get('definitionOfDone'):
                      print("**Definition of done:**")
                      for d in s['definitionOfDone']:
                          print(f"- [ ] {d}")
                      print()
                  print("---\n")

          if data.get('sprintPlan'):
              print("## 🏃 Sprint Plan\n")
              for phase, items in data['sprintPlan'].items():
                  print(f"**{phase}:**")
                  for i in (items if isinstance(items, list) else [items]):
                      print(f"- {i}")
                  print()

          print("---\n*Auto-generated by Claude*")
          PYEOF

          DATE=$(date -u +%Y-%m-%d)

          # Close old review issues
          gh issue list --label "auto-review" --state open --json number -q '.[].number' | \
            while read -r n; do gh issue close "$n" --comment "Superseded." 2>/dev/null || true; done

          # Create new issue
          gh issue create \
            --title "🏗️ Architecture Review — ${DATE}" \
            --body-file /tmp/issue-body.md \
            --label "auto-review"

      - name: Upload artifacts
        if: always()
        uses: actions/upload-artifact@v4
        with:
          name: review-${{ github.run_number }}
          path: |
            /tmp/context.txt
            /tmp/review.json
            /tmp/issue-body.md
          retention-days: 30
