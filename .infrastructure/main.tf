// Create project using vars and SOU project naming conventions
locals {
    project_id = "ut-${var.agency}-${var.application}-${var.environment}"
}

resource "google_project_service" "service" {
  for_each           = toset(var.services)
  service            = each.key
  project            = local.project_id
  disable_on_destroy = false
}

resource "google_compute_global_address" "ingress_ip" {
  project     = local.project_id
  name        = "${var.cluster_name}-ingress"
  description = "Application ingress public IP address"
}

// costs money
module "gke_cluster" {
  source            = "./modules/gke"
  project_id        = local.project_id
  cluster_name      = var.cluster_name
  master_cidr_block = var.master_cidr_block
  max_pods_per_node = var.max_pods_per_node
  gke_machine_type  = var.gke_machine_type
  vpc_self_link     = google_compute_network.agrc_vpc.self_link
  subnet_self_link  = google_compute_subnetwork.agrc_subnet.self_link
  pod_subnet        = var.pod_subnet.range_name
  service_subnet    = var.service_subnet.range_name
}

resource "google_compute_router" "router" {
  name    = "${local.project_id}-nat-router"
  project = local.project_id
  region  = google_compute_subnetwork.agrc_subnet.region
  network = google_compute_network.agrc_vpc.self_link

  bgp {
    asn = 65025
  }
}

resource "google_compute_router_nat" "nat" {
  name                               = "${local.project_id}-nat"
  project                            = local.project_id
  region                             = google_compute_router.router.region
  router                             = google_compute_router.router.name
  nat_ip_allocate_option             = "AUTO_ONLY"
  source_subnetwork_ip_ranges_to_nat = "ALL_SUBNETWORKS_ALL_IP_RANGES"

  log_config {
    enable = true
    filter = "ERRORS_ONLY"
  }
}
