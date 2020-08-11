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
