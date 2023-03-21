# readme

Execute these commands from this folder to create a mock for 326 east south temple, slc

```bash
curl -X POST localhost:8081/mocks --header "Content-Type: application/x-yaml" --data-binary "@AddressPoints.findAddressCandidates.yml"
curl -X POST localhost:8081/mocks --header "Content-Type: application/x-yaml" --data-binary "@Roads.findAddressCandidates.yml
curl -X POST localhost:8081/mocks --header "Content-Type: application/x-yaml" --data-binary "@AddressPoints.findAddressCandidates.empty.yml"
curl -X POST localhost:8081/mocks --header "Content-Type: application/x-yaml" --data-binary "@Roads.findAddressCandidates.empty.yml"
curl -X POST localhost:8081/mocks --header "Content-Type: application/x-yaml" --data-binary "@Roads.reverseGeocode.yml"
curl -X POST localhost:8081/mocks --header "Content-Type: application/x-yaml" --data-binary "@Roads.healthCheck.yml"
curl -X POST localhost:8081/mocks --header "Content-Type: application/x-yaml" --data-binary "@AddressPoints.healthCheck.yml"
curl -X POST localhost:8081/mocks --header "Content-Type: application/x-yaml" --data-binary "@Geometry.healthCheck.yml"
```

A success message looks like the following

```json
{ "message": "Mocks registered successfully" }
```
