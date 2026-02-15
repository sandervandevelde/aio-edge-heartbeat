# aio-edge-heartbeat

This solution offers a Azure IoT Operations module for a heartbeat.

## Azure IoT Edge equivalent

The goal is to create a solution similar to the Azure IoT Edge module.

The config.json is compatible to the Azure IoT Edge tools as seen in the Azure IoT Edge extension for Visual Studio Code. 

## Deployment

The image of the module is deployed as:

```
docker.io/svelde/aio-edge-heartbeat:0.0.1-amd64
```

For deploying this module, a kubectl yaml file must be created:

```
sudo kubectl create namespace nsaioedgeheartbeat
sudo mkdir aioedgeheartbeat
cd aioedgeheartbeat
sudo nano aioedgeheartbeat.yaml
sudo kubectl apply -f aioedgeheartbeat.yaml --namespace=nsaioedgeheartbeat
```

The content of the yaml file is:

```
---
apiVersion: apps/v1
kind: Deployment
metadata:
  name: aioedgeheartbeat
spec:
  replicas: 1
  selector:
    matchLabels:
      app: aioedgeheartbeat
  template:
    metadata:
      labels:
        app: aioedgeheartbeat
    spec:
      containers:
      - name: aioedgeheartbeat
        image: svelde/aio-edge-heartbeat:0.0.1-amd64
```

If needed, the whole module can be deleted at once with:

```
sudo kubectl delete namespace nsaioedgeheartbeat
```

## MIT License

This module is available under a MIT license.