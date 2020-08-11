variable agency {
  type    = string
}

variable application {
  type    = string
}

variable environment {
  type    = string
}

variable region {
  type = string
}

variable roles_kwalker {
  type = list
  default = [
    "roles/owner"
  ]
}


//services to enable
variable services {
  type = list
  default = []
}

// Labels
variable project_labels {
    type = map
    default = {
        app = "void"
        contact    = "void"
        dept       = "void"
        elcid      = "void"
        env        = "void"
        security   = "0"
        supportcode = "void"
  }
}
//VPC
variable primary_subnet {
  type    = string
}
// GKE
variable cluster_name {
  type    = string
}
variable master_cidr_block {
  type  = string
  description = "IP block used for k8s master peering"
}
variable max_pods_per_node {
  type    = number
}
variable gke_machine_type {
  type = string
  description = "Machine type for default GKE node pool"
}
variable pod_subnet {
  type = object({
    range_name    = string
    ip_cidr_range = string
  })
}
variable service_subnet {
  type = object({
    range_name    = string
    ip_cidr_range = string
  })
}
