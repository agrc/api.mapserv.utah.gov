
variable project_id {
  type        = string
  description = "The project ID to manage the Cloud SQL resources"
}

variable region {
  type        = string
  description = "The region of the Cloud SQL resources"
  default     = "us-central1"
}

// GKE
variable "cluster_name" {
  type = string
}
variable master_cidr_block {
  type        = string
  description = "IP block used for k8s master peering"
}
variable max_pods_per_node{
  type        = string
  description = "Max number of pods allowed on a node. Used to apply subnets from the pod range to nodes."
}
variable gke_machine_type {
  type        = string
  description = "Machine type for default GKE node pool"
}
variable "vpc_self_link" {
  type = string
}
variable "subnet_self_link" {
  type = string
}
variable "pod_subnet" {
  type = string
}
variable "service_subnet" {
  type = string
}
