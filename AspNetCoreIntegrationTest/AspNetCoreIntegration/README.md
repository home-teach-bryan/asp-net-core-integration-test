## AspNetCoreIntegration 

## 啟動專案前的準備
1. 確認專案內的`docker-compose.yaml`，以及執行`docker compose up -d`建立SQL Server
2. 執行`dotnet ef database update` 讓ef tool根據Migrations還原資料庫schema
3. 連線至資料庫，確認資料庫有還原成功
