environment="testing"
location="EastUS"
locationPrefix="east-us"
clusterUniqueName="p14mtesteus"
useMssqlElasticPool=false

cd "$(dirname "${BASH_SOURCE[0]}")"
. ../deploy.sh
