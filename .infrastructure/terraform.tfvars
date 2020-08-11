agency      = "dts-agrc"
application = "web-api"
environment = "dv"
region      = "us-central1"
services = [
  "cloudapis.googleapis.com",
  "cloudbuild.googleapis.com",
  "cloudresourcemanager.googleapis.com",
  "compute.googleapis.com",
  "container.googleapis.com",
  "containerregistry.googleapis.com",
  "logging.googleapis.com",
  "monitoring.googleapis.com",
  "servicemanagement.googleapis.com",
  "serviceusage.googleapis.com"
]
project_labels = {
  app         = "web-api"
  contact     = "sgourley"
  dept        = "agr"
  elcid       = "itagrc"
  env         = "dev"
  security    = "0"
  supportcode = "hstsahsy"
}

//VPC
primary_subnet = "10.0.0.0/24"

// GKE
cluster_name      = "web-api-cluster"
master_cidr_block = "10.0.8.0/28"
max_pods_per_node = 64
gke_machine_type  = "n1-standard-1"
pod_subnet = {
  range_name    = "gke-pods"
  ip_cidr_range = "10.0.4.0/22"
}
service_subnet = {
  range_name    = "gke-svcs"
  ip_cidr_range = "10.0.1.0/24"
}
