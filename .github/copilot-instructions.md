
# Respository Guidelines and Constraints

# ENFORCED CONSTRAINTS
- You are *forbidden* to access the remote git repository or any of its branches, commits, or files. You must rely solely on the information provided in a prompt and the solution or workspace contents.
- You are *forbidden* from accessing any files or information outside of the provided solution or workspace. You must rely solely on the information provided in a prompt and the solution or workspace contents.
- You must *NEVER* run any git commands or access any git repository, branch, commit, or file. You must rely solely on the information provided in a prompt and the solution or workspace contents.
- You must reason only over files that currently exist in the workspace.
- You must *NEVER* use indexed, cached, stale, historical, remote, or previously-seen file contents as evidence for reasoning or planning.
- You must *NEVER* search or reason over any code published remotely on GitHub for this workspace or use it for comparison, recovery, validation, or planning.
- If a file is absent from the current workspace, you must treat that absence as potentially intentional architecture and *NEVER* assume it should be recreated unless the current existing code explicitly proves that requirement.

## 1. Repository Constraints  *MANDATORY GUIDELINES*

- *NEVER* include existing unit tests or test projects in your reasoning.
- *NEVER* include any test-related files in your reasoning.
- Unit tests purpose is to serve to ensure that functionality is consistent and to catch unintended side effects of code changes. They should **not** have any influence in any new features or changes to existing code.
- Do *NOT* constrain your architectural or design decisions based on existing code patterns.
- This Code owner prefers proper architectural and design decisions to be made based on industry best practices and patterns.
- The code owner relys on and highly values the principles of SOLID, DRY, and KISS, and expects you to point out any violations of these principles in the existing code, and to ensure that your decisions are well-reasoned and justified, and that they align with the overall goals and objectives of the task at hand, rather than being influenced by existing code patterns or styles in the repository.
- When making architectural or design decisions, consider the principles of SOLID, DRY, and KISS, as well as the specific requirements and constraints of the project.
- Ensure that your decisions are well-reasoned and justified, and that they align with the overall goals and objectives of the task at hand, rather than being influenced by existing code patterns or styles in the repository.
- When assessing the repository for adding or editing features, focus on the architectural and design aspects of the code, rather than being influenced by existing code patterns or styles.
- Before presenting conclusions, plans, or architectural recommendations, you must verify every material claim against the current on-disk files involved in the active code path.
- Any result from broad search, indexing, cache, memory, or earlier exploration must be treated as provisional until confirmed by directly reading the current existing workspace files.
- Deleted, missing, renamed, or relocated files must not be treated as defects by default; validate whether the current design intentionally removed or replaced them before proposing recreation.

## 1. Structure and Organization

- The repository is organized into three main projects:
  - `MarkdownViewer.Wpf` - the core control library that provides the markdown rendering functionality.
  - `MarkdownViewer.Wpf.Sample` - a sample WPF application that demonstrates how to use the control library.
  - `MarkdownViewer.Wpf.Tests` - a project containing automated tests for the control library.

  ### 1.1 Structural Guidelines

  - Methods should be shaped to allow for clear separation of concerns, maintainability, and testability.
  - The public API should be designed to be intuitive and easy to use for developers integrating the markdown viewer into their applications.
  - Internal methods should be structured to facilitate unit testing and maintainability, while keeping implementation details hidden from the public API.

## 2. Coding Style

- Methods should be internal by default, and only public if they are part of the public API or are required to be public for other reasons (e.g., event handlers, interface implementations).
-

## 3. Unit Testing - *MANDATORY GUIDELINES*

- Unit tests serve as a regression safety net to ensure that functionality is consistent and to catch unintended side effects of code changes.
- Unit tests **should not** have any influence in any new features or changes to existing code.
- All tests will be managed separately from any other development tasks, and should not be considered when assessing the repository for adding or editing features.

- DO NOT create, edit, or run tests unless explicitly ask to.
- DO NOT include existing unit tests or test projects in your reasoning.
- DO NOT include any test-related files in your reasoning.
- Unit tests should be created and run only when explicitly asked to, and should not be considered when assessing the repository for adding or editing features.
- When asked to create unit tests, ensure they are comprehensive and cover all relevant scenarios for the feature being tested.
- Tests should be targeting the public API and InternalsVisibleTo members of the library, and should not be testing private implementation details.
