# SumiUI のデザインから DebugToolkit が取り込めそうな要素

調査対象: [POPOPOinc/SumiUI](https://github.com/POPOPOinc/SumiUI)（Unity UI Toolkit ランタイム UI 向けのダークテーマ、スタイルシートのみ / C# なし / 依存なし / MIT）

比較対象: `Packages/jp.andantetribe.debugtoolkit/ExternalResources/DebugToolkitUss.uss`（単一ファイル・520 行）と `DefaultRuntimeTheme.tss`

---

## 0. 前提の違い（そのまま真似してはいけない部分）

| | SumiUI | DebugToolkit |
| --- | --- | --- |
| 目的 | プロダクトのランタイム UI 全体をテーマする | ゲーム画面に**重ねる**デバッグオーバーレイ |
| セレクタ | `.unity-button` など素の Unity クラスを直接上書き（= 全体に効く） | すべて `.debug-toolkit__master` 配下にスコープ（= ゲーム UI を汚さない） |
| 提供物 | USS/TSS のみ。ユーティリティクラスは利用者が自分で付ける | C# 拡張メソッドで組み立て。利用者は USS を書かない |

DebugToolkit のスコープ方針とC#ファーストのコンセプトは維持すべきで、SumiUI から取るのは
**「トークン設計」「ファイル構成」「状態（focus/disabled）の作り込み」「カバレッジ」** の 4 領域。

---

## 1. スタイルシートの分割と `@import` 構成（優先度: 高）

SumiUI は `Styles/Parts/` 以下に責務ごとに分割し、`Sumi.uss` で束ねている。

```
Sumi.uss
  @import Parts/Variables.uss   ← トークン定義のみ
  @import Parts/Base.uss        ← Label / BaseField（全体の土台）
  @import Parts/Fields.uss
  @import Parts/Buttons.uss
  @import Parts/Sliders.uss
  @import Parts/Containers.uss  ← Foldout / Box / TabView / SplitView
  @import Parts/Collections.uss ← ListView / MultiColumnView / TreeView
  @import Parts/Utilities.uss   ← opt-in の sumi-* クラス
```

冒頭コメントに `Import order is load-bearing: Base first, Containers before Collections.` と
**順序に意味があることを明記**しているのが良い。

現状 DebugToolkit は 520 行の単一 USS で、コメント区切りだけで分けている。
同じ粒度（Variables / Base / Fields / Buttons / Sliders / Containers / Window / Console）に割って
`DebugToolkitUss.uss` を `@import` 束ね役にすると、差分レビューと衝突が楽になる。
`.tss` 側の構成（`unity-theme://default` を先に import してから自前 USS）は既に両者同じ。

## 2. トークンの二層化: プリミティブ + セマンティック（優先度: 高）

SumiUI の `Variables.uss`:

```css
/* 1層目: グレースケールランプ（00 = #000 〜 15 = #fff の 16 段） */
--sumi-grayscale-00 … --sumi-grayscale-15;

/* 2層目: 意味づけ */
--sumi-panel-bg-color: rgba(0, 0, 0, 0.8);
--sumi-focus:  #9bd;
--sumi-accent: #4ad;
--sumi-selected-bg: #146;

/* 寸法 */
--sumi-font-size: 12px;
--sumi-control-height: 24px;
--sumi-label-width: 160px;
--sumi-input-width: 140px;
--sumi-group-width: 300px;
```

DebugToolkit は色が 7 個のフラットな変数（background / lowlight / black / highlight / content / true / false）しかなく、
**トークンから漏れた生値が各所に残っている**:

- `border-color: rgb(113, 31, 31)`（Toggle off の縁）
- `border-color: rgb(62, 154, 62)`（Toggle on の縁）
- `background-color: rgba(246, 137, 30, 0.7)`（toggle-all ボタン = highlight の 70%）
- `border-color: gray`（console-view）
- C# 側にも: `ConsoleView.cs` の `new Color(0f, 0.68f, 0.71f)` は実質 `--debug-toolkit-color-content`、
  `DebugConst.StyleColor.Warning / Error` も USS トークンと二重管理

取り込み案:
- ハイライトの濃淡（hover 用 / 縁用 / 半透明用）を `--debug-toolkit-color-highlight-dark` のように
  トークン化し、生の `rgb()` を全廃する
- そうすると `:root` の変数を利用者側 USS で上書きするだけで**フォークせずに再テーマ可能**になる
  （= 「デバッグメニューの色をプロジェクトのブランドに合わせたい」に無改造で応えられる）
- 色トークンは README に公開 API として表を書く（SumiUI がユーティリティクラス表を出しているのと同じ扱い）

## 3. コントロール寸法のトークン化（優先度: 中）

SumiUI は `--sumi-control-height: 24px` を Button / Popup / RadioButton / MultiColumnHeader が共有し、
高さが揃う。DebugToolkit は spacing と radius はトークン化済みだが高さ系がバラバラ:

- `--debug-toolkit-tab-height: 30px`（タブだけ定義済み）
- スライダーの dragger `height: 24px` / min-max の thumb `24px`
- ラジオの `width/height: 24px`、チェックマーク `12px`
- delete / minimize ボタン `20px`、toggle-all `30px`
- `.unity-base-field__label { min-width: 80px }`

`--debug-toolkit-control-height` / `--debug-toolkit-label-width` / `--debug-toolkit-icon-size` を足して
これらを差し替えると、「コンパクト版」を変数上書きだけで作れる。

## 4. `:root` に `font-size` と `color` を置く（優先度: 低）

SumiUI は `:root` 自体に `font-size` と `color` を指定して継承させ、各ルールでの色指定を減らしている。
DebugToolkit は `.debug-toolkit__master` に `font-size: 12px` を置き、`color:` を各ルールで繰り返している。
スコープ方針上 `:root` には置けないが、`.debug-toolkit__master` に `color` を一度置けば
`.unity-label` などの重複指定を減らせる。

## 5. `:focus` / `:disabled` / `:hover:enabled` の作り込み（優先度: 高）

SumiUI は状態を丁寧に持っている:

```css
.unity-base-field:focus:enabled > .unity-base-field__label { color: var(--sumi-focus); }
.unity-radio-button .unity-radio-button__input:focus:enabled .unity-radio-button__checkmark-background {
    border-color: var(--sumi-focus);
}
.unity-base-text-field:disabled .unity-base-text-field__input { … }
.unity-button:hover:enabled { … }
```

現状の `DebugToolkitUss.uss` には **`:focus` ルールが 1 つもなく、`:disabled` も無い**。
また `:hover` / `:active` に `:enabled` が付いていないため、無効化した要素もホバー反応する。

デバッグメニューは実機でゲームパッド / キーボードから触ることが多く、フォーカスリングが無いと
どこを選択中か分からない。`--debug-toolkit-color-focus` を足して
Button / TextField / Toggle / Popup / Foldout にフォーカス表現を入れるのが投資対効果が高い。

## 6. カバレッジの穴（優先度: 高、特にドロップダウン）

SumiUI がカバーしていて DebugToolkit の USS に無いコントロール:

| コントロール | 備考 |
| --- | --- |
| `.unity-base-dropdown__container-inner` | **ドロップダウンを開いた時のリスト本体**。DebugToolkit は `unity-popup-field` / `unity-enum-field` の閉じた見た目だけを当てているので、開くとデフォルトテーマの見た目に戻り、テーマが破綻する。しかもポップアップは `.debug-toolkit__master` の**外**に生成されるため、現在のスコープ付きセレクタでは絶対に当たらない。非スコープのルールか専用クラス付与が要る |
| CompositeField / BoundsField | Vector2/3/4Field。デバッグメニューでは座標いじりで頻出 |
| HelpBox / GroupBox | 警告表示に使える |
| ListView footer / size-field | `AddConsoleView` は ListView ベースなので効いてくる |
| MultiColumnView（ヘッダ・ソート矢印） | 将来テーブル表示を足す時に |
| TreeView | 同上 |
| TwoPaneSplitView（dragline のホバー色） | ウィンドウ分割に使うなら |
| CollectionView の交互背景 / drag-hover バー | ログ一覧の可読性に直結 |

また SumiUI は `EnumField` が popup 系クラスを持たない Unity の仕様を
「セレクタをカンマで並べて両方書く」で処理している。DebugToolkit は popup-field と enum-field で
ほぼ同一のブロックを 2 回書いているので、同じ手でまとめられる。

## 7. opt-in ユーティリティクラス（優先度: 中 / C# 拡張メソッドとして）

SumiUI の `sumi-*`（自動では付かない、名前空間衝突しない接頭辞）:

| クラス | 内容 |
| --- | --- |
| `sumi-group` | 固定幅カラム。box を縦に積む |
| `sumi-box` | 角丸 + 枠 + 半透明のセクション |
| `sumi-row` | 横並び + 折り返し |
| `sumi-header` | 太字 + 下線のセクション見出し |
| `sumi-button-group` | `RadioButtonGroup` をセグメンテッドコントロール化（チェックマークを消し、等幅でボタン見た目にする） |
| `sumi-unit-label` | `px` / `deg` / `%` をフィールド右端に薄色でピン留め（`picking-mode="Ignore"` 必須） |

DebugToolkit のコンセプト（利用者に USS を書かせない）を守るなら、
**クラスではなく C# 拡張メソッドとして同じ機能を出す**のが筋:

- `root.AddRow()` … 横並びコンテナ（`sumi-row` 相当）。今は縦積み一択
- `root.AddHeader("Player")` … セクション見出し（`sumi-header` 相当）
- `AddSegmentedControl(params string[])` … `sumi-button-group` 相当。モード切替に強い。
  現状ラジオボタンは縦リストのみ
- `field.AddUnitLabel("ms")` … `sumi-unit-label` 相当。ProfileInfoLabel やスライダー値と相性が良い

## 8. 半透明・ダーク・高密度というオーバーレイ的な選択（優先度: 中、要議論）

SumiUI は `--sumi-panel-bg-color: rgba(0, 0, 0, 0.8)` で**下のゲーム画面を透かす**。
`control-height 24px` / `font-size 12px` / `padding 4px` / `margin 0` とかなり詰めている。

DebugToolkit は不透明のクリーム色 `rgb(255, 250, 234)`、`margin 8px`、タブ 30px、
`--debug-toolkit-panel-min-width: 400px` と、ゆったり＆明るい。
これは DebugToolkit 独自の「見やすさ・押しやすさ（実機タップ前提）」の判断なので置き換える必要はないが、

- **オーバーレイ中にゲーム画面が隠れる**問題は SumiUI 側の解が有効
- 変数だけで切り替わる `compact` / `translucent` バリアント（別 tss を 1 枚足す）を用意すると、
  「小さい画面でデバッグ表示が邪魔」というよくある不満に答えられる

## 9. ドキュメントとサンプルの作り（優先度: 中）

- SumiUI の README は**ユーティリティクラスを表で列挙**し、各クラスの用途と注意（`picking-mode="Ignore"` など）まで書いている。
  DebugToolkit の README は C# API を網羅している一方、**USS 変数によるカスタマイズ手段が未記載**。2. と合わせて表を足したい
- SumiUI は `Assets/Samples/SumiGallery.unity` に全コントロールを並べたギャラリーシーンを置いている。
  DebugToolkit は既に CI で Unity のスクリーンショットテストを回しているので、
  **全コントロールを並べたギャラリーサンプル = USS のビジュアル回帰テスト**として二重に効く
- 名前の由来（墨 / 隅）のような短いコンセプト説明が README 冒頭にあると、テーマの意図が伝わりやすい

---

## 付録: 突き合わせ中に見つかった現行 USS の不具合

SumiUI との比較とは独立に、`DebugToolkitUss.uss` に効いていないルールが 2 箇所ある。

1. `debug-toolkit__toggle-all-button:hover`（先頭のドット抜け）
   → 型セレクタ扱いになり何にもマッチしない。`.debug-toolkit__toggle-all-button:hover` が正
2. `.debug-toolkit__master.unity-base-slider--horizontal .unity-text-field #unity-text-input`
   （`__master` と `.unity-base-slider--horizontal` の間のスペース抜け）
   → master 要素自身がスライダーであることはないのでマッチしない。直前のルールと同じく子孫セレクタが正

また `DebugConst` が定義しているのに USS に対応ルールが無いクラスがある:
`__window-label` / `__toggle-window-display` / `__safe-area-container` / `__master-window` / `__normal-window`。
C# 側の判別用途なら問題ないが、`__window-label` はスタイルを当てる前提の命名に見える。

---

## 優先度まとめ

| 優先 | 項目 |
| --- | --- |
| 高 | 生値の全廃 + セマンティックトークン化（2）、`:focus` / `:disabled` 対応（5）、ドロップダウンのポップアップ本体（6）、Parts 分割（1） |
| 中 | 寸法トークン（3）、横並び / 見出し / セグメンテッドコントロールの拡張メソッド（7）、compact・半透明バリアント（8）、README のトークン表 + ギャラリーサンプル（9） |
| 低 | `color` の継承整理（4） |
