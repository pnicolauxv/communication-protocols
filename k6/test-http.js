import http from 'k6/http';

export const options = {
  vus: 500,
  duration: '30s',
};

export default function () {
  // http.get('http://localhost:5000/test?protocol=http');
  http.get('http://servicea-service-pablo-demo.apps.demo-xv-skvbd.msp.crossvale.com/test?protocol=http&size=medium');
}