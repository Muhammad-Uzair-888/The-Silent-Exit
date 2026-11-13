# AI Agents Guidelines

## Context rules
- Do not read *.csproj, *.sln, Library/, Temp/, Logs/, obj/, or UserSettings/.
- Work mainly in Assets/_Project/Scripts, Assets/_Project/Scenes, Assets/_Project/Art, and tasks/.
- Ask before reading any other folder.

## Unity rules
- Treat this repository as a Unity project first.
- Prefer Unity MCP for validation and editor-safe automation. Do not use dotnet commands to validate.
- Do not hand-author or edit .meta files. Unity generates them.

## Input rules
- Scripts use the Legacy Input System (Input Manager).
- Exception: Assets/_Project/ThirdParty/ModularFirstPersonController uses the legacy Input Manager. Do not edit its scripts, and do not rewrite its input code.

## Project rules
- Gameplay follows the GDD. Ask before changing its rules, scope, or room layout.
- Level walls come from the kit prefabs. Do not use ProBuilder or primitive cubes for walls.
- Never mark a task done without verifying it in Unity or the Console.

## Lessons
- After any correction from the user, add the pattern to tasks/lessons.md and write a rule that prevents it.