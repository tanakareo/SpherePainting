## SpherePainting
このツールは、巴山竜来研究室（xMath Lab）と画家の[山本雄基](https://yamamotoyuki.com/)さんとの共同プロジェクトで制作した絵画制作支援ツールです。
球をシーン上にランダムに配置し、そのプレビューやPNG画像への出力ができます。球の生成の仕方、球の色などを細かくパラメータで調節できます。
画像作成の流れは[この動画](https://www.youtube.com/watch?v=HW7pB9QWP5I)を参照してください。
## ユーザーインターフェース
![Image](https://github.com/user-attachments/assets/6806af54-840e-4d7a-8629-f9869b79af02)
<br />
### ビューポート
| アイコン | 説明 |
|:---:|:---|
| <img src="https://github.com/user-attachments/assets/65ed334a-2862-45c1-9f6f-51339aba2581" height="30px vertical-align:bottom"> | ビューポートのカメラとレンダリング時のカメラを切り替えます。 |
### サイドバー
「球の生成」、「球の質感」、「シーンのレンダリング」、「レンダリング」、「キャンバス」、「ファイル」の６項目があります。
| アイコン | 説明 |
|:---:|:---|
| <img src="https://github.com/user-attachments/assets/15a89497-2051-41d1-9ff4-5433cdce4358" height="30px vertical-align:bottom"> | **球の生成** <br /> 球の半径、普通の球、差集合、共通部分の球の割合など |
| <img src="https://github.com/user-attachments/assets/139f34b4-866d-47fb-9909-2f78a69b3004" height="30px vertical-align:bottom"> | **球の質感** <br />  球の色、色のブレンドモード（乗算、オーバーレイ）など |
| <img src="https://github.com/user-attachments/assets/2317ebae-6d71-4413-945b-4bc14588f64b" height="30px vertical-align:bottom"> | **シーンのレンダリング** <br /> 背景の色、空間の歪み（開発中） |
| <img src="https://github.com/user-attachments/assets/d724f0ac-4f1a-45be-8f27-1850564040ec" height="30px vertical-align:bottom"> | **レンダリング** <br /> カメラの設定、レンダリングの実行、結果の保存 |
| <img src="https://github.com/user-attachments/assets/4ee7069b-9306-49a2-8d1f-7d4902191cbd" height="30px vertical-align:bottom"> | **キャンバス** <br /> キャンバスの直径、奥行き、キャンバスの質感など |
| <img src="https://github.com/user-attachments/assets/2ae0f392-b4ea-4a35-bcdf-a7324c7d2ac8" height="30px vertical-align:bottom"> | **ファイル** <br /> プロジェクトの保存、PNG画像へのエクスポートなど |
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
