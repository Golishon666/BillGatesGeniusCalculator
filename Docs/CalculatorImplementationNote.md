# Bill Gates Genius Calculator - Internal Implementation Note

## Architecture

The application follows the PDF requirement: Clean Architecture with MVP.

- `Calculator.Domain` contains pure expression evaluation and state models. It has no Unity references.
- `Calculator.Application` contains the presenter and ports for view, persistence, and dialog interaction.
- `Calculator.Infrastructure` contains the Unity JSON repository backed by `Application.persistentDataPath`.
- `Calculator.Unity` contains only Unity-facing composition and calculator views.
- `MessageBox` is an isolated reusable dialog module.

Only View classes inherit from `MonoBehaviour`. Each View has an explicit `Initialize` method. Presenters, repositories, services, models, and startup classes are plain C# classes.

## Behavior

The calculator supports addition of non-negative integer operands separated by `+`.

Valid examples: `54+21`, `45+00`, `1+2+3`.

Invalid examples: empty input, spaces, decimal numbers, minus, division, leading/trailing plus, repeated plus, and overflow.

On success the presenter inserts a newest-first history entry like `5+5=10`, clears input, refreshes the view, and saves state.

On error the presenter inserts `expression=ERROR`, clears input, shows the message box with `Please check the expression you just entered`, then restores the failed expression after `GOT IT`.

## Prefabs

All UI is prefab-backed:

- `CalculatorApp.prefab` owns Canvas, EventSystem, black background, screen prefab instance, message box prefab instance, and the VContainer `CalculatorAppView`.
- `CalculatorScreen.prefab` owns the title, input field, result button, and history scroll area.
- `HistoryItem.prefab` owns one right-aligned blue history text row.
- `MessageBox.prefab` owns the modal panel and `GOT IT` button.

Runtime UI creation is limited to instantiating `HistoryItem.prefab` for history rows and binding text values.

## Dependency Injection

`CalculatorAppView` derives from `LifetimeScope` and is treated as the root View. It registers domain services, repository, presenters, dialog service, and View instances with VContainer.

`CalculatorStartup` is a non-MonoBehaviour entry point. It calls every View `Initialize` method and starts the presenter restore flow.

## Verification

EditMode tests cover expression evaluation, presenter flows, persistence fallback, and architecture rules:

- only `*View` types inherit `MonoBehaviour`;
- every MonoBehaviour View exposes `Initialize`;
- presenter success/error flows match the PDF scenario;
- state is restored from JSON and corrupted files fall back safely.
