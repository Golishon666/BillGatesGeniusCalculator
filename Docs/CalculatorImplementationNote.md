# Bill Gates Genius Calculator - Internal Implementation Note

## Architecture

The application follows the PDF requirement: Clean Architecture with MVP.

- `Calculator.Domain` contains pure expression evaluation, state models, and the serializable operation flags model. It has no Unity references.
- `Calculator.Application` contains the presenter and ports for view, persistence, and dialog interaction.
- `Calculator.Infrastructure` contains the Unity JSON repository backed by `Application.persistentDataPath`.
- `Calculator.Unity` contains only Unity-facing composition and calculator views.
- `MessageBox` is an isolated reusable dialog module.

Only View classes inherit from `MonoBehaviour`. Each View has an explicit `Initialize` method. Presenters, repositories, services, models, and startup classes are plain C# classes.

## Behavior

The calculator supports addition of non-negative integer operands separated by `+`.

Valid examples: `54+21`, `45+00`, `1+2+3`.

Invalid examples: empty input, spaces, decimal numbers, minus, division, leading/trailing plus, repeated plus, and overflow.

Enabled operations are controlled by the serializable `CalculatorOperationsConfig`. The prefab default keeps only `Addition = true`; `Subtraction`, `Multiplication`, and `Division` are present as disabled flags for future expansion. The input field does not filter characters while typing, matching the PDF scenario: validation happens only when the user presses `RESULT`, and `AdditionExpressionEvaluator` rejects disabled operations and unsupported characters at the domain level.

On success the presenter inserts a newest-first history entry like `5+5=10`, clears input, refreshes the view, and saves state.

On error the presenter inserts `expression=ERROR`, clears input, shows the message box with `Please check the expression you just entered`, then restores the failed expression after `GOT IT`.

Debug: `Space` clears the full history when the input field is not focused (`ClearHistoryDebug` on the presenter).

## Prefabs

All UI is prefab-backed:

- `CalculatorApp.prefab` owns Canvas, EventSystem, black background, screen prefab instance, message box prefab instance, and the VContainer `CalculatorAppView`.
- `CalculatorScreen.prefab` owns the TextMeshPro title, `TMP_InputField`, result button, history scroll area, and the thin right-side history scrollbar.
- History is rendered by one right-aligned multiline `TextMeshProUGUI` inside the scroll area. `CalculatorScreenView` builds the text with `StringBuilder`, including line breaks and the visual indentation needed by the reference.
- `MessageBox.prefab` owns the modal panel and `GOT IT` button.

Runtime UI does not generate calculator controls. It only updates prefab-owned components and toggles the history scroll area when entries exist.

Reference empty-state geometry is authored in `CalculatorScreen.prefab` for the 1080x1920 portrait target. The screen does not use Unity layout components.

- calculator panel, title, input, and `InputLine` stay at prefab-authored positions; only the panel background grows downward when history appears, keeping its top edge fixed so the header and input do not move;
- when history appears, `CalculatorScreenView` shows `HistoryScroll`, extends the panel, and shifts the `RESULT` button downward by the visible history height plus `resultButtonHistoryOffset`;
- `maxVisibleHistoryRows` defaults to `7` on `CalculatorScreenView`; row height is derived from `historyText` preferred line height;
- input field is visually transparent: no box, no filled background, left-aligned text, italic grey placeholder `Enter an equation...`, and a separate thin underline below it;
- input underline (`InputLine`) colors are prefab-serialized as `inputLineDefaultColor` and `inputLineActiveColor` on `CalculatorScreenView`;
- history viewport uses `RectMask2D` for clipping; mouse wheel and drag scrolling are enabled only when the history scrollbar is visible (more than `maxVisibleHistoryRows` entries);
- the history scrollbar is a prefab-owned uGUI `Scrollbar` under `HistoryScroll`; `CalculatorScreenView` toggles it when history exceeds `maxVisibleHistoryRows`, while `ScrollRect` drives handle size and scrolling after layout rebuild;

## Dependency Injection

`CalculatorAppView` derives from `LifetimeScope` and is treated as the root View. It registers domain services, repository, presenters, dialog service, and View instances with VContainer.

`CalculatorStartup` is a non-MonoBehaviour entry point. It calls every View `Initialize` method and starts the presenter restore flow.

## Verification

EditMode tests cover expression evaluation, presenter flows, persistence fallback, and architecture rules:

- only `*View` types inherit `MonoBehaviour`;
- every MonoBehaviour View exposes `Initialize`;
- presenter success/error flows match the PDF scenario;
- state is restored from JSON and corrupted files fall back safely.
