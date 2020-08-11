resource "google_compute_global_address" "ingress_ip" {
  project     = var.project_id
  name        = "${var.cluster_name}-ingress"
  description = "Application ingress public IP address"
}

// GKE
resource "google_container_cluster" "dev_cluster" {
  provider = google-beta
  project  = var.project_id
  name     = var.cluster_name
  location = var.region
  node_locations = [
    "${var.region}-a",
    "${var.region}-b"
  ]
  network    = var.vpc_self_link
  subnetwork = var.subnet_self_link
  release_channel {
    channel = "REGULAR"
  }

  //this can be in google_container_node_pool resource instead
  #remove_default_node_pool = true # use this when deleting default node pools
  initial_node_count        = 1
  default_max_pods_per_node = var.max_pods_per_node

  private_cluster_config {
    enable_private_nodes    = true
    enable_private_endpoint = false
    master_ipv4_cidr_block  = var.master_cidr_block
  }

  node_config {
    machine_type = var.gke_machine_type
    disk_size_gb = "100"
    metadata = {
      disable-legacy-endpoints = "true"
    }
    oauth_scopes = [
      "https://www.googleapis.com/auth/logging.write",
      "https://www.googleapis.com/auth/monitoring",
      "https://www.googleapis.com/auth/devstorage.read_only",
      "https://www.googleapis.com/auth/cloud_debugger",
      "https://www.googleapis.com/auth/service.management",
      "https://www.googleapis.com/auth/servicecontrol"
    ]
  }
  ip_allocation_policy {
    services_secondary_range_name = var.service_subnet
    cluster_secondary_range_name  = var.pod_subnet
  }
}
