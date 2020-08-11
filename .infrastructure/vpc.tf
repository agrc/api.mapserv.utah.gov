resource "google_compute_network" "agrc_vpc" {
  name                    = "ut-${var.agency}-${var.application}-${var.environment}-vpc"
  auto_create_subnetworks = "false"
  description             = "VPC with IP range"
}

resource "google_compute_subnetwork" "agrc_subnet" {
  name                     = "ut-${var.agency}-${var.application}-${var.environment}-subnet"
  ip_cidr_range            = var.primary_subnet
  region                   = var.region
  network                  = google_compute_network.agrc_vpc.self_link
  private_ip_google_access = true

  secondary_ip_range = [{
    range_name    = var.service_subnet.range_name
    ip_cidr_range = var.service_subnet.ip_cidr_range
  },
  {
    range_name    = var.pod_subnet.range_name
    ip_cidr_range = var.pod_subnet.ip_cidr_range
  }]
}
