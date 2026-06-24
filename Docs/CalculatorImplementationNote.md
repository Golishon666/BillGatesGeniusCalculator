# Bill Gates Genius Calculator - Internal Implementation Note

## Architecture

The application follows the PDF requirement: Clean Architecture with MVP.

- `Calculator` is the standalone calculator assembly. Its folders still separate domain, application, infrastructure, and Unity composition code.
- `MessageBox` is the standalone reusable dialog assembly and does not reference calculator code.
- `CalculatorMessageBoxBridge` is the optional composition assembly that subscribes to calculator message dialog events and calls the public `MessageBoxLifetimeScope.ShowAsync` API.
- `Calculator.Tests` is the editor-only test assembly.

Views inherit from `MonoBehaviour`; module entry points inherit from VContainer `LifetimeScope`. Each View has an explicit `Initialize` method. Presenters, repositories, services, models, and startup classes are plain C# classes.

## Behavior

The calculator supports addition of non-negative integer operands separated by `+`.

Valid examples: `54+21`, `45+00`, `1+2+3`.

Invalid examples: empty input, spaces, decimal numbers, minus, division, leading/trailing plus, repeated plus, and overflow.

Enabled operations are controlled by the `CalculatorOperationsConfig` ScriptableObject asset. The prefab default references the asset with only `Addition = true`; `Subtraction`, `Multiplication`, and `Division` are present as disabled flags for future expansion. The input field does not filter characters while typing, matching the PDF scenario: validation happens only when the user presses `RESULT`, and `AdditionExpressionEvaluator` rejects disabled operations and unsupported characters at the domain level.

On success the presenter inserts a newest-first history entry like `5+5=10`, clears input, refreshes the view, and saves state.

On error the presenter inserts `expression=ERROR`, clears input, shows the message box with `Please check the expression you just entered`, then restores the failed expression after `GOT IT`.

Debug: `Space` clears the full history when the input field is not focused. Handled by `CalculatorHistoryDebugView` (separate from `CalculatorScreenView`); calls `ClearHistoryDebug` on the presenter.

## Prefabs

All UI is prefab-backed:

- `CalculatorApp.prefab` owns Canvas, black background, screen prefab instance, and the VContainer `CalculatorAppView`.
- `CalculatorScreen.prefab` owns the TextMeshPro title, `TMP_InputField`, result button, history scroll area, and the thin right-side history scrollbar.
- History is rendered by one right-aligned multiline `TextMeshProUGUI` inside the scroll area. `CalculatorScreenView` builds the text with `StringBuilder`, including line breaks and the visual indentation needed by the reference.
- `MessageBox.prefab` owns the modal panel and `GOT IT` button; `Canvas-MessageBox.prefab` wraps it with a Canvas and `MessageBoxLifetimeScope` for standalone UI placement.
- `CalculatorMessageBoxBridge.prefab` owns the composition `LifetimeScope` and contains both child prefabs: `CalculatorApp.prefab` and `Canvas-MessageBox.prefab`. Use it when the calculator should show errors through the message box module.

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

`CalculatorAppView` derives from `LifetimeScope` and is the calculator module scope. It registers domain services, repository, presenters, the event-backed dialog service, and View instances with VContainer.

`CalculatorStartup` is a non-MonoBehaviour entry point. It calls every View `Initialize` method and starts the presenter restore flow.

`MessageBoxLifetimeScope` is the message box module scope. It registers `MessageBoxViewConfig`, `IMessageBoxView`, `IMessageBoxPresenter`, initializes `MessageBoxView`, and exposes `ShowAsync(message, buttonText)` for composition code.

`CalculatorMessageBoxLifetimeScope` is the composition scope. It keeps serialized references to the calculator and message box child scopes; its startup subscribes to `CalculatorAppView.MessageDialogRequested` and forwards requests to `MessageBoxLifetimeScope.ShowAsync`.

## Verification

EditMode tests cover expression evaluation, presenter flows, persistence fallback, and architecture rules:

- in the calculator assembly, only `*View` types inherit `MonoBehaviour`;
- every calculator MonoBehaviour View exposes `Initialize`;
- presenter success/error flows match the PDF scenario;
- `MessageDialogEventService` completes silently without subscribers and awaits subscribers when the bridge is present;
- state is restored from JSON and corrupted files fall back safely.

Assembly boundaries are intentional: `Calculator` does not reference `MessageBox`, `MessageBox` does not reference `Calculator`, and `CalculatorMessageBoxBridge` is the only assembly that references both.
