terraform {
  backend "gcs" {
    bucket  = "ut-dts-agrc-web-api-dev-tfstate"
    prefix  = "state"
  }
}
