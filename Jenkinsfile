pipeline{
    agent any
    
    environment {
        dotnet ='C:\\Program Files (x86)\\dotnet\\'
        }
        
    triggers {
        pollSCM 'H * * * *'
    }
    
    stages{
      stage('Checkout') {
        steps {
          git url: 'https://github.com/javatec-as/asp-net-core-example-app.git/', branch: 'master'
        }
      }
      
      stage('Restore packages') {
        steps {
          bat "dotnet restore WebApplication.sln"
        }
      }
      
      stage('Build') {
        steps{
          bat "dotnet build WebApplication\\WebApplication.csproj --configuration Release"
        }
      }
      
      stage('Test: Unit Test') {
        steps {
          bat "dotnet test XUnitTestProject\\XUnitTestProject.csproj"
        }
      }
    }
 }