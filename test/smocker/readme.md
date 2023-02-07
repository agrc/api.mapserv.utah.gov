# readme

Execute these commands to create a mock for 326 east south temple, slc

```bash
curl -X POST localhost:8081/mocks --header "Content-Type: application/x-yaml" --data-binary "@AddressPoints.findAddressCandidates.yml"
curl -X POST localhost:8081/mocks --header "Content-Type: application/x-yaml" --data-binary "@Roads.findAddressCandidates.yml"
```
