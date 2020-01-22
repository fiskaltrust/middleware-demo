use chrono::prelude::*;
use qrcode::QrCode;
use read_input::prelude::*;
use read_input::shortcut::with_description;
use restson::{Error, RestClient, RestPath};
use rust_decimal::Decimal;
use rust_decimal_macros::*;
use serde::{Deserialize, Serialize};
use url::Url;

#[allow(non_snake_case)]
#[derive(Serialize, Deserialize, Debug, Clone)]
struct FtChargeItem {
  Quantity: Decimal,
  Description: String,
  Amount: Decimal,
  VATRate: Decimal,
  ftChargeItemCase: i64,
  ftChargeItemCaseData: Option<String>,
  VATAmount: Option<Decimal>,
  AccountNumber: Option<String>,
  CostCenter: Option<String>,
  ProductGroup: Option<String>,
  ProductNumber: Option<String>,
  ProductBarcode: Option<String>,
  Unit: Option<String>,
  UnitQuantity: Option<Decimal>,
  UnitPrice: Option<Decimal>,
  Moment: Option<DateTime<Utc>>,
}
impl Default for FtChargeItem {
  fn default() -> FtChargeItem {
    FtChargeItem {
      Quantity: dec!(0),
      Description: String::new(),
      Amount: dec!(0),
      VATRate: dec!(0),
      ftChargeItemCase: 0,
      ftChargeItemCaseData: None,
      VATAmount: None,
      AccountNumber: None,
      CostCenter: None,
      ProductGroup: None,
      ProductNumber: None,
      ProductBarcode: None,
      Unit: None,
      UnitQuantity: None,
      UnitPrice: None,
      Moment: None,
    }
  }
}

#[allow(non_snake_case)]
#[derive(Serialize, Deserialize, Debug, Clone)]
struct FtPayItem {
  Quantity: Decimal,
  Description: String,
  Amount: Decimal,
  ftPayItemCase: i64,
  ftPayItemCaseData: Option<String>,
  VATAmount: Option<Decimal>,
  AccountNumber: Option<String>,
  CostCenter: Option<String>,
  MoneyGroup: Option<String>,
  MoneyNumber: Option<String>,
  Moment: Option<DateTime<Utc>>,
}
impl Default for FtPayItem {
  fn default() -> FtPayItem {
    FtPayItem {
      Quantity: dec!(0),
      Description: String::new(),
      Amount: dec!(0),
      ftPayItemCase: 0,
      ftPayItemCaseData: None,
      VATAmount: None,
      AccountNumber: None,
      CostCenter: None,
      MoneyGroup: None,
      MoneyNumber: None,
      Moment: None,
    }
  }
}

#[allow(non_snake_case)]
#[derive(Serialize, Deserialize, Debug, Clone)]
struct FtSignRequest {
  ftCashBoxID: String,
  ftQueueID: Option<String>,
  ftPosSystemId: Option<String>,
  cbTerminalID: String,
  cbReceiptReference: String,
  cbReceiptMoment: DateTime<Utc>,
  cbChargeItems: Vec<FtChargeItem>,
  cbPayItems: Vec<FtPayItem>,
  ftReceiptCase: i64,
  ftReceiptCaseData: Option<String>,
  cbReceiptAmount: Option<Decimal>,
  cbUser: Option<String>,
  cbArea: Option<String>,
  cbCustomer: Option<String>,
  cbSettlement: Option<String>,
  cbPreviousReceiptReference: Option<String>,
}
impl Default for FtSignRequest {
  fn default() -> FtSignRequest {
    FtSignRequest {
      ftCashBoxID: String::new(),
      ftQueueID: None,
      ftPosSystemId: None,
      cbTerminalID: String::new(),
      cbReceiptReference: String::new(),
      cbReceiptMoment: Utc::now(),
      cbChargeItems: Vec::new(),
      cbPayItems: Vec::new(),
      ftReceiptCase: 0,
      ftReceiptCaseData: None,
      cbReceiptAmount: None,
      cbUser: None,
      cbArea: None,
      cbCustomer: None,
      cbSettlement: None,
      cbPreviousReceiptReference: None,
    }
  }
}
impl RestPath<()> for FtSignRequest {
  fn get_path(_: ()) -> Result<String, Error> {
    Ok(String::from("json/sign"))
  }
}

#[allow(non_snake_case)]
#[derive(Serialize, Deserialize, Debug)]
struct FtSignature {
  ftSignatureFormat: i64,
  ftSignatureType: i64,
  caption: Option<String>,
  data: Option<String>,
}
impl Default for FtSignature {
  fn default() -> FtSignature {
    FtSignature {
      ftSignatureFormat: 0,
      ftSignatureType: 0,
      caption: None,
      data: None,
    }
  }
}

#[allow(non_snake_case)]
#[derive(Serialize, Deserialize, Debug)]
struct FtSignResponse {
  ftCashBoxID: String,
  ftQueueID: String,
  ftQueueItemID: String,
  ftQueueRow: i64,
  cbTerminalID: String,
  cbReceiptReference: String,
  ftCashBoxIdentification: String,
  ftReceiptIdentification: String,
  ftReceiptMoment: DateTime<Utc>,
  ftReceiptHeader: Option<Vec<String>>,
  ftChargeItems: Option<Vec<FtChargeItem>>,
  ftChargeLines: Option<Vec<String>>,
  ftPayItems: Option<Vec<FtPayItem>>,
  ftPayLines: Option<Vec<String>>,
  ftSignatures: Vec<FtSignature>,
  ftReceiptFooter: Option<Vec<String>>,
  ftStateData: Option<String>,
  ftState: i64,
}

#[allow(non_snake_case)]
#[derive(Serialize, Deserialize, Debug)]
struct BelegeGruppe {
  Signaturzertifikat: String,
  Zertifizierungsstellen: Vec<String>,
  #[serde(rename = "Belege-kompakt")]
  BelegeKompakt: Vec<String>,
}

#[allow(non_snake_case)]
#[derive(Serialize, Deserialize, Debug)]
struct FtJournalResponse {
  #[serde(rename = "Belege-Gruppe")]
  BelegeGruppe: Vec<BelegeGruppe>,
}

#[allow(non_snake_case)]
#[derive(Serialize, Deserialize, Debug)]
struct FtJournalRequest {}

impl RestPath<()> for FtJournalRequest {
  fn get_path(_: ()) -> Result<String, Error> {
    Ok(String::from("json/journal"))
  }
}

enum FtCountryCode {
  AT,
  FR,
  DE
}
impl FtCountryCode {
  fn value(&self) -> i64 {
    match *self {
      FtCountryCode::AT => 0x4154_0000_0000_0000i64,
      _ => 0
    }
  }
}

fn main() -> Result<(), Box<dyn std::error::Error>> {
  let service_url = input::<Url>()
    .err_match(with_description)
    .repeat_msg("service url: ")
    .get();

  let country_code: FtCountryCode = match service_url.domain().unwrap().split('.').collect::<Vec<&str>>().last() {
    Some(&"at") => FtCountryCode::AT,
    Some(&"fr") => FtCountryCode::FR,
    Some(&"de") => FtCountryCode::DE,
    _ => {
      let mut tmp: Option<FtCountryCode> = None;
      while tmp.is_none() {
        tmp = match input::<String>().msg("country code:").get().trim() {
          "at" => Some(FtCountryCode::AT),
          "fr" => Some(FtCountryCode::FR),
          "de" => Some(FtCountryCode::DE),
          default => {
            println!("unknown country code {}", default);
            None
          }
        }
      }
      tmp.unwrap()
    }
  };

  let cashboxid: String = input().msg("cashboxid: ").get();

  let accesstoken: String = input().msg("accesstoken: ").get();

  let mut client = RestClient::new(service_url.as_str())?;

  client.set_header("Content-Type", "application/json")?;
  client.set_header("cashboxid", &cashboxid.trim())?;
  client.set_header("accesstoken", &accesstoken.trim())?;

  let mut ft_sign_request: Vec<FtSignRequest>;

  let ft_normal_receipt = FtSignRequest {
    ftCashBoxID: cashboxid.clone(),
    ftQueueID: None,
    ftPosSystemId: None,
    cbTerminalID: String::from("1"),
    cbReceiptReference: String::from("cbReceiptReference"),
    cbReceiptMoment: Utc::now(),
    cbChargeItems: vec![
      FtChargeItem {
        Quantity: dec!(1),
        Description: String::from("Artikel 1"),
        Amount: dec!(4.8),
        VATRate: dec!(20),
        ftChargeItemCase: country_code.value(),
        ..Default::default()
      },
      FtChargeItem {
        Quantity: dec!(1),
        Description: String::from("Artikel 2"),
        Amount: dec!(3.3),
        VATRate: dec!(20),
        ftChargeItemCase: country_code.value(),
        ..Default::default()
      },
    ],
    cbPayItems: vec![FtPayItem {
      ftPayItemCase: country_code.value(),
      Amount: dec!(4.8) + dec!(3.3),
      Quantity: dec!(1),
      Description: String::from("Bar"),
      ..Default::default()
    }],
    ftReceiptCase: country_code.value(),
    ..Default::default()
  };
  let ft_zero_receipt = FtSignRequest {
    ftCashBoxID: cashboxid.clone(),
    cbTerminalID: String::from("1"),
    cbReceiptReference: String::from("1"),
    cbReceiptMoment: Utc::now(),
    cbChargeItems: Vec::new(),
    cbPayItems: Vec::new(),
    ftReceiptCase: country_code.value() | 0x0002,
    ..Default::default()
  };

  loop {
    ft_sign_request = Vec::new();
    println!("1: Barumsatz (0x4154000000000001)");
    println!("2: Null-Beleg (0x4154000000000002)");
    println!("3: Inbetriebnahme-Beleg (0x4154000000000003)");
    println!("4: Außerbetriebnahme-Beleg (0x4154000000000004)");
    println!("5: Monats-Beleg (0x4154000000000005)");
    println!("6: Jahres-Beleg (0x4154000000000006)");

    println!("9: RKSV-DEP export");
    println!("10: Anzahl der zu sendenden Barumsatzbelege (max MAX_GENERATED_RECEIPT_COUNT)");

    println!("exit: Program beenden");

    let action: String = input().msg("choice: ").get();

    match action.trim() {
      "1" => {
        ft_sign_request.push(ft_normal_receipt.clone());
      }
      "2" => {
        ft_sign_request.push(ft_zero_receipt.clone());
      }
      "3" => {
        let mut ft_tmp_receipt = ft_zero_receipt.clone();
        ft_tmp_receipt.ftReceiptCase = country_code.value() | 0x0003;
        ft_sign_request.push(ft_tmp_receipt.clone());
      }
      "4" => {
        let mut ft_tmp_receipt = ft_zero_receipt.clone();
        ft_tmp_receipt.ftReceiptCase = country_code.value() | 0x0004;
        ft_sign_request.push(ft_tmp_receipt.clone());
      }
      "5" => {
        let mut ft_tmp_receipt = ft_zero_receipt.clone();
        ft_tmp_receipt.ftReceiptCase = country_code.value() | 0x0005;
        ft_sign_request.push(ft_tmp_receipt.clone());
      }
      "6" => {
        let mut ft_tmp_receipt = ft_zero_receipt.clone();
        ft_tmp_receipt.ftReceiptCase = country_code.value() | 0x0006;
        ft_sign_request.push(ft_tmp_receipt.clone());
      }
      "9" => {
        let dep: FtJournalResponse = client.post_capture_with(
          (),
          &FtJournalRequest {},
          &[
            ("type", format!("{}", country_code.value() | 0x0001).as_str()),
            ("from", "0"),
            ("to", "0"),
          ],
        )?;
        println!("{:#?}", dep);
      }
      "exit" => std::process::exit(0),
      default => {
        let n: u32 = match default.parse() {
          Ok(result) => result,
          Err(error) => {
            println!("invalid option \"{}\" {:?}", action, error);
            continue;
          }
        };

        for i in 0..n {
          let mut ft_tmp_receipt = ft_normal_receipt.clone();
          ft_tmp_receipt.cbReceiptReference = (i + 1).to_string();
          ft_sign_request.push(ft_tmp_receipt.clone());
        }
      }
    }

    for s in ft_sign_request.into_iter() {
      let ft_sign_response: FtSignResponse = client.post_capture((), &s)?;

      ft_sign_response.ftSignatures.iter().for_each(|s| {
        if let Some(data) = &s.data {
          if s.ftSignatureFormat == 0x0003 {
            if let Some(caption) = &s.caption {
              println!("{}", caption);
            }

            println!(
              "{}",
              QrCode::new(data.as_bytes())
                .unwrap()
                .render::<char>()
                .quiet_zone(false)
                .module_dimensions(2, 1)
                .build()
            );
          }
        }
      });

      println!("{:#?}", ft_sign_response);
    }
  }
}
