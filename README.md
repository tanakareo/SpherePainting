SpherePaintingのUnityプロジェクトです。バージョンはUnity 6000.0.32f1です。
## SpherePainting
このツールは、巴山竜来研究室（xMath Lab）と[山本雄基](https://yamamotoyuki.com/)さんとの共同プロジェクトで制作した絵画制作支援ツールです。
<br />
<br />
球をシーン上にランダムに配置し、そのプレビューやPNG画像への出力ができます。球の生成の仕方、球の色などを細かくパラメータで調節できます。
画像作成の流れは[この動画](https://www.youtube.com/watch?v=HW7pB9QWP5I)を参照してください。
## ユーザーインターフェース
![Image](https://github.com/user-attachments/assets/a2216983-3db4-4eca-999e-b35ff3ec4d3c)
<br />
### ビューポート
| アイコン | 説明 |
|:---:|:---|
| <img src="https://github.com/user-attachments/assets/297ed9b5-c221-4a7f-8f4f-f45f65dc74de" height="40px vertical-align:bottom"> | ビューポートのカメラとレンダリング時のカメラを切り替えます。 |
### サイドバー
「球の生成」、「球の質感」、「シーンのレンダリング」、「レンダリング」、「キャンバス」、「ファイル」の６項目があります。
| アイコン | 説明 |
|:---:|:---|
| <img src="https://github.com/user-attachments/assets/31c77a62-0220-4d41-891c-ca5b2b29159e" height="40px vertical-align:bottom"> | **球の生成** <br /> 球の半径、普通の球、差集合、共通部分の球の割合など |
| <img src="https://github.com/user-attachments/assets/1ec20c27-d69f-4cb1-851b-14eece08a9e8" height="40px vertical-align:bottom"> | **球のレンダリング** <br />  球の色、色のブレンドモード（乗算、オーバーレイ）など |
| <img src="https://github.com/user-attachments/assets/7c4534d5-bfd0-4ca0-89cf-08285d7bed6c" height="40px vertical-align:bottom"> | **シーンのレンダリング** <br /> 背景の色、空間の歪み（開発中） |
| <img src="https://github.com/user-attachments/assets/768b3b08-44e8-4916-b394-bd4159244e65" height="40px vertical-align:bottom"> | **レンダリング** <br /> カメラの設定、レンダリングの実行、結果の保存 |
| <img src="https://github.com/user-attachments/assets/7f4066eb-54da-441e-8615-df57ad64c464" height="40px vertical-align:bottom"> | **キャンバス** <br /> キャンバスの直径、奥行き、キャンバスの質感など |
| <img src="https://github.com/user-attachments/assets/0bcf762f-125f-411d-964b-25d57a295356" height="40px vertical-align:bottom"> | **ファイル** <br /> プロジェクトの保存、PNG画像へのエクスポートなど |
### ツールバー
| アイコン | 説明 |
|:---|:---|
| <img src="https://github.com/user-attachments/assets/7900df43-c588-47fc-bf5b-8dc7988a0c8e" height="30px vertical-align:bottom"> | モードの切り替えを行います。左が編集モード、右がキャンバスモードです。 |
| <img src="https://github.com/user-attachments/assets/0a2033bc-9322-4242-b94c-1921a00ab7d2" height="30px vertical-align:bottom"> | ギズモの表示を設定します。右の下矢印からギズモの透明度など詳細な設定ができます。 |
## 操作方法
### 視点操作
| 操作 | 機能 |
| ------------- | ------------- |
| マウス右ドラッグ  | 視点の平行移動  |
| マウス左ドラッグ  | 視点の回転  |
| マウスホイール | ズーム |
| SPACE | 視点のリセット |
