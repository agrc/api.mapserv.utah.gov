extern crate reqwest;
extern crate serde_json;

mod models;

use models::{GeocodeResult, ResultContainer};
use std::env;

#[derive(serde::Serialize)]
#[allow(dead_code)]
enum Locators {
    All,
    AddressPoints,
    RoadCenterlines,
}
impl std::fmt::Display for Locators {
    fn fmt(&self, f: &mut std::fmt::Formatter) -> std::fmt::Result {
        match self {
            Locators::All => write!(f, "all"),
            Locators::AddressPoints => write!(f, "addressPoints"),
            Locators::RoadCenterlines => write!(f, "roadCenterlines"),
        }
    }
}

#[derive(serde::Serialize)]
#[allow(dead_code)]
enum Format {
    Esrijson,
    Geojson,
}
impl std::fmt::Display for Format {
    fn fmt(&self, f: &mut std::fmt::Formatter) -> std::fmt::Result {
        match self {
            Format::Esrijson => write!(f, "esrijson"),
            Format::Geojson => write!(f, "geojson"),
        }
    }
}

#[derive(serde::Serialize)]
struct GeocodingOptions {
    accept_score: Option<i32>,
    pobox: Option<bool>,
    locators: Option<Locators>,
    suggest: Option<i8>,
    spatial_reference: Option<i32>,
    format: Option<Format>,
    callback: Option<String>,
}
impl Default for GeocodingOptions {
    fn default() -> Self {
        Self {
            accept_score: None,
            pobox: None,
            locators: None,
            suggest: None,
            spatial_reference: None,
            format: None,
            callback: None,
        }
    }
}

struct GeocodeService {
    api_key: String,
}

impl GeocodeService {
    fn new(api_key: String) -> Self {
        Self { api_key }
    }

    async fn locate(
        &self,
        street: &str,
        zone: &str,
        options: GeocodingOptions,
    ) -> Result<ResultContainer<Option<GeocodeResult>>, reqwest::Error> {
        let url = format!(
            "https://api-1057643351324.us-central1.run.app/api/v1/geocode/{}/{}",
            street, zone
        );

        let response: ResultContainer<Option<GeocodeResult>> = reqwest::Client::new()
            .get(url)
            .query(&[("apiKey", self.api_key.clone())])
            .query(&options)
            .send()
            .await?
            .json()
            .await?;

        // Return the response
        Ok(response)
    }
}

#[tokio::main]
async fn main() {
    let api_key = env::var("UGRC_API_KEY").expect("UGRC_API_KEY must be set");
    let geocode = GeocodeService::new(api_key);

    let result = geocode
        .locate(
            "123 south main street",
            "slc",
            GeocodingOptions {
                accept_score: Some(90),
                spatial_reference: Some(4326),
                ..Default::default()
            },
        )
        .await;

    println!("{:#?}", result);
}
