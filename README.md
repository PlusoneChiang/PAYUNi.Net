# 目錄
* [環境需求](#環境需求)
* [安裝](#安裝)
* [使用方式](#使用方式)
# 環境需求
* .NET 8
# 安裝
* 請加入PayuniSDK參考至專案

# 使用方式
* 正式區
```csharp
payuniAPI payuniapi = new payuniAPI(key, iv);
```
* 測試區
```csharp
payuniAPI payuniapi = new payuniAPI(key, iv, EnviromentType.SandBox);
```
* API串接
```csharp
string result = payuniApi.UniversalTrade(encryptInfo, mode);
```
* upp ReturnURL、NotifyURL接收到回傳參數後處理方式
```csharp
string result = payuniApi.ResultProcess(requestData);
```
* 參數說明
  * EncryptInfoModel encryptInfo
    * 參數詳細內容請參考[統一金流API串接文件](https://www.payuni.com.tw/docs/web/#/7/34)對應功能請求參數的EncryptInfo
```csharp=
EncryptInfoModel encryptInfo = new EncryptInfoModel();
encryptInfo.MerID = "ABC";
encryptInfo.Timestamp= DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
```
  * string merKey
    * 請登入PAYUNi平台檢視商店串接資訊取得 Hash Key
  * string merIV
    * 請登入PAYUNi平台檢視商店串接資訊取得 Hash IV
  * Enum type (非必填，若無填入則為預設值)
    ```csharp=
    public enum EnviromentType
    {
        // 連線測試區
        SandBox,
        // 連線正式區 (default)
        Production
    }
    ```
  * Enum TradeType:
    ```csharp=
    public enum TradeType
    {
        // 交易建立 整合式
        Upp,
        // 交易建立 虛擬帳號幕後
        Atm,
        // 交易建立 超商代碼幕後
        Cvs,
        // 交易建立 Line Pay 幕後
        Linepay,
        // 交易建立 Aftee 幕後
        AfteeDirect,
        // 交易建立 信用卡幕後
        Credit,
        // 交易請退款
        TradeClose,
        // 交易取消授權
        TradeCancel,
        // 後支付確認(Aftee)
        TradeConfirmAftee,
        // 交易取消超商代碼(Cvs)
        CancelCvs,
        // 信用卡Token取消(約定/記憶卡號)
        CreditBindCancel,
        // 愛金卡退款(Icash)
        TradeRefundIcash,
        // 後支付退款(Aftee)
        TradeRefundAftee,
        // Line Pay退款(Line)
        TradeRefundLinepay,
        // 交易查詢
        TradeQuery,
        // 信用卡Token查詢(約定)
        CreditBindQuery,
    }
    ```
* 其餘請參考[範例](https://github.com/payuni/NET_SDK/tree/main/example)

* 原生C#
```csharp=
using PayuniSDK;
string merKey = "12345678901234567890123456789012";
string merIV = "1234567890123456";
payuniAPI payuni = new payuniAPI(key, iv);
EncryptInfoModel encryptInfo = new EncryptInfoModel();
encryptInfo.MerID = "ABC";
encryptInfo.TradeNo = "16614190477810373246";
encryptInfo.Timestamp= DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
string result = payuniApi.UniversalTrade(encryptInfo, TradeType.TradeQuery);
```
# LICENSE
```text
Copyright 2022 PRESCO. All rights reserved.
Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at
    http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
```
