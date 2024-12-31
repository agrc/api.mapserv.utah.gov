extern crate serde;

use serde::Deserialize;

#[derive(Debug, Deserialize)]
#[allow(dead_code)]
pub struct ResultContainer<T> {
    pub status: i32,
    pub message: Option<String>,
    pub result: T,
}

#[derive(Debug, Deserialize)]
#[allow(dead_code)]
pub struct GeocodeResult {
    pub location: Location,
    pub score: f64,
    pub locator: String,
    #[serde(rename = "matchAddress")]
    pub match_address: Option<String>,
    #[serde(rename = "inputAddress")]
    pub input_address: String,
    #[serde(rename = "standardizedAddress")]
    pub standardized_address: Option<String>,
    #[serde(rename = "addressGrid")]
    pub address_grid: String,
    pub candidates: Option<Vec<Candidate>>,
}

#[derive(Debug, Deserialize)]
#[allow(dead_code)]
pub struct Candidate {
    address: String,
    pub location: Location,
    pub score: f64,
    pub locator: String,
    #[serde(rename = "addressGrid")]
    pub address_grid: String,
}

#[derive(Debug, Deserialize)]
#[allow(dead_code)]
pub struct Location {
    pub x: f64,
    pub y: f64,
}
