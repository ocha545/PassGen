# PassGen Ver0.0.1
パスワードを生成するコマンドです

# 出来ること
- パスワードを生成する数の指定
- パスワードの文字数の指定
- パスワードから除外する文字の指定 (ダブルクオーテーションとシングルクオーテーションは最初から除外しています)

# ビルド
- ``dotnet publish -c Release`` でビルドします
- それ以外のOSでは
- ``dotnet publish -c Release -r linux-x64``　や、
- ``dotnet publish -c Release -r osx-x64`` などでビルドします
- ビルドされたファイルは、``bin/Release/net10.0/publish``ディレクトリに出力されます


# 使い方
- ``PassGen --help`` または ``PassGen /?``などでヘルプを表示します
- ``PassGen --count <number>`` で生成するパスワードの数を指定します (デフォルトでは1)
- ``PassGen --length <number>`` で生成するパスワードの文字数を指定します (デフォルトでは16)
- ``PassGen --exclusion <text>`` でパスワードに含めたくない文字を指定します (デフォルトではダブルクオーテーションとシングルクオーテーション)
-- それぞれ組み合わせて使うことが出来ます
