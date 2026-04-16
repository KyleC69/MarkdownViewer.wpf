
# Respository Guidelines and Constraints

## 1. Repository Constraints  *MANDATORY GUIDELINES*

- *NEVER* include existing unit tests or test projects in your reasoning.
- Do *NOT* constrain your architectural or design decisions based on existing code patterns.
- This Code owner prefers proper architectural and design decisions to be made based on industry best practices and patterns, rather than being constrained by existing code patterns or styles in the repository.
- When making architectural or design decisions, consider the principles of SOLID, DRY, and KISS, as well as the specific requirements and constraints of the project.
- Ensure that your decisions are well-reasoned and justified, and that they align with the overall goals and objectives of the task at hand, rather than being influenced by existing code patterns or styles in the repository.
- When assessing the repository for adding or editing features, focus on the architectural and design aspects of the code, rather than being influenced by existing code patterns or styles.

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

*Testing Philosophy* - Unit tests serve as a regression safety net to ensure that functionality is consistent and to catch unintended side effects of code changes. Unit tests **should not** have any influence in any new features or changes to existing code. All tests will be managed separately from any other development tasks, and should not be considered when assessing the repository for adding or editing features.

- DO NOT create, edit, or run tests unless explicitly ask to.
- DO NOT include existing unit tests or test projects in your reasoning.
- DO NOT include any test-related files in your reasoning.
- Unit tests should be created and run only when explicitly asked to, and should not be considered when assessing the repository for adding or editing features.
- When asked to create unit tests, ensure they are comprehensive and cover all relevant scenarios for the feature being tested.
- Tests should be targeting the public API and InternalsVisibleTo members of the library, and should not be testing private implementation details.