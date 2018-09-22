1. Install minikube with choco  
`choco install minikube`

2. start minikube  
`minikube start --vm-driver="hyperv" --memory=4096 --alsologtostderr`

3. start dashboard  
`minikube dashboard`

4. set environment var for runnig the shell in the VM  
`minikube docker-env --shell powershell`

5. set shell to shell in VM  
`minikube docker-env | Invoke-Expression`

At this point we have Minikube installed and Powershell configured to run inside the VM with K8s running on it.

6. CD to the root of the project    
`cd .\SoftwareProjects\akka-cluster-issue\`

7. build an image inside the VM for the version with .NET Core 2.0 and Akka.NET 1.3.8  
 `docker build --rm --no-cache -t cluster-issue-netcore20-akka138:0.0.1 .\src\SimpleCluster_NetCore20_Akka138\`

8. start cluster in Minikube using the image we created in the previous step  
`kubectl apply -f .\yaml\cluster-issue-netcore20-akka138.yaml`

9. follow logs on the first pod with our image running on it  
`kubectl logs cluster-seed-netcore20-akka138-0 -f`

You should see a healthy cluster with three members up.

10. build image for .NET Core 2.0 and Akka.NET 1.3.9  
`docker build --rm --no-cache -t cluster-issue-netcore21-akka139:0.0.1 .\src\SimpleCluster_NetCore21_Akka139\`

11. start a separate cluster using this image   
`kubectl apply -f .\yaml\cluster-issue-netcore21-akka139.yaml`

12. follow the logs on the first pod with the image we just created  
`kubectl logs cluster-seed-netcore21-akka139-0 -f`

The cluster does not form, we tested this in Minikube and on our K8s cluster in GCP and we saw similar results each time. These issues do not manifest when we are developing on our Windows 10 laptops.