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

## Prefabs

All UI is prefab-backed:

- `CalculatorApp.prefab` owns Canvas, EventSystem, black background, screen prefab instance, message box prefab instance, and the VContainer `CalculatorAppView`.
- `CalculatorScreen.prefab` owns the TextMeshPro title, `TMP_InputField`, result button, history scroll area, and the thin right-side history scrollbar.
- History is rendered by one right-aligned multiline `TextMeshProUGUI` inside the scroll area. `CalculatorScreenView` builds the text with `StringBuilder`, including line breaks and the visual indentation needed by the reference.
- `MessageBox.prefab` owns the modal panel and `GOT IT` button.

Runtime UI does not generate calculator controls. It only updates prefab-owned components and toggles the history scroll area when entries exist.

Reference empty-state geometry is tuned for the 1080x1920 portrait target. The screen does not use Unity layout components; `CalculatorScreenView` updates prefab-owned `RectTransform`s directly.

- calculator panel: `720x480`, anchored at `(0, -130)`;
- title/result/input placeholder TMP font sizes: `24/40/40`;
- input field is visually transparent: no box, no filled background, left-aligned text, italic grey placeholder `Enter an equation...`, and a separate thin light-grey underline below it;
- result button preferred height: `96`;
- input underline preferred height: `4`.
- history adds up to six visible rows at `44` px each; the panel grows downward by the visible history height and the result button shifts by the same amount.
- history clipping uses `RectMask2D` on `HistoryScroll/Viewport`; do not replace it with a transparent `Image + Mask`, because that can hide the TextMeshPro history text.
- the history scrollbar is a prefab-owned uGUI `Scrollbar` under `HistoryScroll`; `CalculatorScreenView` toggles it only when history exceeds six visible rows and manually sets the handle size because the calculator does not use Unity layout components.

## Dependency Injection

`CalculatorAppView` derives from `LifetimeScope` and is treated as the root View. It registers domain services, repository, presenters, dialog service, and View instances with VContainer.

`CalculatorStartup` is a non-MonoBehaviour entry point. It calls every View `Initialize` method and starts the presenter restore flow.

## Verification

EditMode tests cover expression evaluation, presenter flows, persistence fallback, and architecture rules:

- only `*View` types inherit `MonoBehaviour`;
- every MonoBehaviour View exposes `Initialize`;
- presenter success/error flows match the PDF scenario;
- state is restored from JSON and corrupted files fall back safely.
