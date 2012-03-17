## 概要

![](http://cdn-ak.f.st-hatena.com/images/fotolife/d/daruyanagi/20120311/20120311000132.png)

**「SoundKeyboard 2012」**は、キーのタイプで音を鳴らすタスクトレイ常駐型ソフトです((もともとは @subsfn 氏が Delphi で制作したものですが、だいぶ古くなったので C# で作り直しました。))。

## おもな機能

![](http://cdn-ak.f.st-hatena.com/images/fotolife/d/daruyanagi/20120311/20120311001450.png)

主要な機能は以下のとおりです。

* ミュート機能（［Ctrl］＋［Alt］＋［M］キー）
* デスクトップに入力キーを表示する機能
* サウンドパックの切り替え機能

Windows 7 64bit版でのみ動作を確認しています。

## サウンドパックについて

![](http://cdn-ak.f.st-hatena.com/images/fotolife/d/daruyanagi/20120311/20120311001437.png)

キーにサウンドを割り当てるには、<b>サウンドパック</b>を作成します。といっても大仰なものではなくて、単にフォルダへWAVEファイルを入れておくだけでです。

例えば、「サウンド」フォルダに「A.wav」を入れて、それをサウンドパックに指定すると、［A］キーを押した時に「A.wav」が再生されます。キーの名前はキー入力のデスクトップ表示機能を利用して確認しながらつけていくとイイと思います。サウンドパックの名前は、フォルダと同じです。さきの例で言えば、「サウンド」がそのままサウンドパックの名前になります。

デフォルトでは2つのサウンドパックを収録しています。

*   **alpha**：アルファベットキーを打つと音がなります。
*   **mari_skb**：スペースやエンターなどを押すと音がなります。

音声を作成してくれました北村真里さんに感謝いたします。

### 注意事項

*   [[Notice]] .NET Framework 4 Client Profile が必要です。インストール時にセットアップされます。
*   [[important]] 一部ブラウザーがインストーラーを不正なファイルとして検出します。別に怪しい挙動を仕込んではいませんが、気になる方はダウンロードを控えていただけますようお願いいたします。 - [オレの作ったアプリが不正なファイル呼ばわりされる件について - だるろぐ](http://daruyanagi.hatenablog.com/entry/2012/03/07/221611)

## ToDo または今後の実装予定

*   <del>タスクトレイアイコンの追加</del>
*   <del>ユーザーインターフェイスを何とかする</del>
*   <del>デフォルトサウンド機能</del>
*   <del>キーを押し続けた場合の処理</del>（Reactive Extension？――はとりあえず使わなかった）
*   <del>二重起動抑止</del>
*   <del>最小化状態での起動</del>
*   <del>サウンドパックの保存フォルダの指定</del>
*   設定の保存機能（本体の設定が保存されない）
*   キー入力表示のカスタマイズ

## 変更履歴

* 2.0.0（12/03/15）
    *   **WPFで新規作り直し**
	*   ［追加］設定の自動保存機能
	*   ［追加］キー入力表示のカスタマイズ
	*   ［削除］ミュート機能のショートカット
	*   ［修正］メインウィンドウが表示されない不具合の修正（Hotfix）
* 1.6.0.13（12/03/12）
    *   ［修正］長時間利用しているとフックが無効になる不具合 - [なんでフック、すぐに死んでまうん…… - だるろぐ](http://daruyanagi.hatenablog.com/entry/2012/03/12/004612)
    *   ［修正］KeyDisplayFormがOSのシャットダウンを妨げる不具合
    *   ［改善］MutexがGCで回収されないように修正
* 1.5.0.12（12/03/10）
    *   ［改善］ユーザーインターフェイスの改善（バージョン表記）
    *   ［追加］デフォルトサウンド機能。サンドが割り当てられていないキーで“Default.wav”を再生
    *   ［追加］アイコンをオリジナルなものに変更
    *   ［仕様変更］内部でクラス構造を変更、リファクタリング
* 1.4.0.12（12/03/10）[[Notice]] ソースのみ公開
    *   ［改善］ユーザーインターフェイスの一新
    *   ［追加］デスクトップにキー入力を表示する機能を追加。設定画面からは削除
    *   ［追加］設定の保存機能。SoundPackフォルダの決め打ち廃止。任意のフォルダをSoundPackとして指定可能に
    *   ［仕様変更］SoundPackListクラスの内部仕様が変更。インターフェイスには影響なし
    *   ［仕様変更］最小化状態での起動
* 1.3.0.11（12/03/09）
    *   ［修正］32bit版環境で実行すると、同梱の64bit DLLがロードできずに、グローバルフックが初期化されない。にもかかわらず Dispose しようとしていたため、アプリケーションが終了できなかった。（null 参照の不具合の修正）
    *   ［仕様変更］同梱のDLLを32bit版に差し替えてビルド。32bit版Windowsでも動作するかもしれない（未検証）
* 1.2.0.10（12/03/09）
    *   ミュート機能の追加（ホットキーは［Ctrl］＋［Shift］＋［M］キー）
    *   キーを押し続けた際の挙動を改善。装飾キー・スペース・エンター・バックスペース・デリートの各キーは、音が連続して再生しないように
    *   SoundPack/SoundPackListクラスの追加。メニューとコンボボックスの内容が同期するように改善
* 1.1.0.9（12/03/09）
    * タスクトレイアイコンの追加
    * 二重起動抑止
* 1.0.0.6（12/03/08）
    * 初回リリース

[[http://daruyanagi.net/SoundKeyboard%202012]]

---

### ダウンロード

ソースコードとClickOnce 形式のインストーラーを公開しています。

#### バイナリ

[[http://download.daruyanagi.net/SoundKeyboard2012/|download.daruyanagi.net]]

#### ソースコード

[[https://github.com/daruyanagi/SoundKeyboard2012/|github.com]]## 概要

![](http://cdn-ak.f.st-hatena.com/images/fotolife/d/daruyanagi/20120311/20120311000132.png)

**「SoundKeyboard 2012」**は、キーのタイプで音を鳴らすタスクトレイ常駐型ソフトです((もともとは @subsfn 氏が Delphi で制作したものですが、だいぶ古くなったので C# で作り直しました。))。

## おもな機能

![](http://cdn-ak.f.st-hatena.com/images/fotolife/d/daruyanagi/20120311/20120311001450.png)

主要な機能は以下のとおりです。

* ミュート機能（［Ctrl］＋［Alt］＋［M］キー）
* デスクトップに入力キーを表示する機能
* サウンドパックの切り替え機能

Windows 7 64bit版でのみ動作を確認しています。

## サウンドパックについて

![](http://cdn-ak.f.st-hatena.com/images/fotolife/d/daruyanagi/20120311/20120311001437.png)

キーにサウンドを割り当てるには、<b>サウンドパック</b>を作成します。といっても大仰なものではなくて、単にフォルダへWAVEファイルを入れておくだけでです。

例えば、「サウンド」フォルダに「A.wav」を入れて、それをサウンドパックに指定すると、［A］キーを押した時に「A.wav」が再生されます。キーの名前はキー入力のデスクトップ表示機能を利用して確認しながらつけていくとイイと思います。サウンドパックの名前は、フォルダと同じです。さきの例で言えば、「サウンド」がそのままサウンドパックの名前になります。

デフォルトでは2つのサウンドパックを収録しています。

*   **alpha**：アルファベットキーを打つと音がなります。
*   **mari_skb**：スペースやエンターなどを押すと音がなります。

音声を作成してくれました北村真里さんに感謝いたします。

### 注意事項

*   [[Notice]] .NET Framework 4 Client Profile が必要です。インストール時にセットアップされます。
*   [[important]] 一部ブラウザーがインストーラーを不正なファイルとして検出します。別に怪しい挙動を仕込んではいませんが、気になる方はダウンロードを控えていただけますようお願いいたします。 - [オレの作ったアプリが不正なファイル呼ばわりされる件について - だるろぐ](http://daruyanagi.hatenablog.com/entry/2012/03/07/221611)

## ToDo または今後の実装予定

*   <del>タスクトレイアイコンの追加</del>
*   <del>ユーザーインターフェイスを何とかする</del>
*   <del>デフォルトサウンド機能</del>
*   <del>キーを押し続けた場合の処理</del>（Reactive Extension？――はとりあえず使わなかった）
*   <del>二重起動抑止</del>
*   <del>最小化状態での起動</del>
*   <del>サウンドパックの保存フォルダの指定</del>
*   設定の保存機能（本体の設定が保存されない）
*   キー入力表示のカスタマイズ

## 変更履歴

* 2.0.0（12/03/15）
    *   **WPFで新規作り直し**
	*   ［追加］設定の自動保存機能
	*   ［追加］キー入力表示のカスタマイズ
	*   ［削除］ミュート機能のショートカット
	*   ［修正］メインウィンドウが表示されない不具合の修正（Hotfix）
* 1.6.0.13（12/03/12）
    *   ［修正］長時間利用しているとフックが無効になる不具合 - [なんでフック、すぐに死んでまうん…… - だるろぐ](http://daruyanagi.hatenablog.com/entry/2012/03/12/004612)
    *   ［修正］KeyDisplayFormがOSのシャットダウンを妨げる不具合
    *   ［改善］MutexがGCで回収されないように修正
* 1.5.0.12（12/03/10）
    *   ［改善］ユーザーインターフェイスの改善（バージョン表記）
    *   ［追加］デフォルトサウンド機能。サンドが割り当てられていないキーで“Default.wav”を再生
    *   ［追加］アイコンをオリジナルなものに変更
    *   ［仕様変更］内部でクラス構造を変更、リファクタリング
* 1.4.0.12（12/03/10）[[Notice]] ソースのみ公開
    *   ［改善］ユーザーインターフェイスの一新
    *   ［追加］デスクトップにキー入力を表示する機能を追加。設定画面からは削除
    *   ［追加］設定の保存機能。SoundPackフォルダの決め打ち廃止。任意のフォルダをSoundPackとして指定可能に
    *   ［仕様変更］SoundPackListクラスの内部仕様が変更。インターフェイスには影響なし
    *   ［仕様変更］最小化状態での起動
* 1.3.0.11（12/03/09）
    *   ［修正］32bit版環境で実行すると、同梱の64bit DLLがロードできずに、グローバルフックが初期化されない。にもかかわらず Dispose しようとしていたため、アプリケーションが終了できなかった。（null 参照の不具合の修正）
    *   ［仕様変更］同梱のDLLを32bit版に差し替えてビルド。32bit版Windowsでも動作するかもしれない（未検証）
* 1.2.0.10（12/03/09）
    *   ミュート機能の追加（ホットキーは［Ctrl］＋［Shift］＋［M］キー）
    *   キーを押し続けた際の挙動を改善。装飾キー・スペース・エンター・バックスペース・デリートの各キーは、音が連続して再生しないように
    *   SoundPack/SoundPackListクラスの追加。メニューとコンボボックスの内容が同期するように改善
* 1.1.0.9（12/03/09）
    * タスクトレイアイコンの追加
    * 二重起動抑止
* 1.0.0.6（12/03/08）
    * 初回リリース

[[http://daruyanagi.net/SoundKeyboard%202012]]

---

### ダウンロード

ソースコードとClickOnce 形式のインストーラーを公開しています。

#### バイナリ

[[http://download.daruyanagi.net/SoundKeyboard2012/|download.daruyanagi.net]]

#### ソースコード

[[https://github.com/daruyanagi/SoundKeyboard2012/|github.com]]