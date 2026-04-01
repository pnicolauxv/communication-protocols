import grpc from 'k6/net/grpc';
import { check } from 'k6';

export const options = {
  vus: 200,
  duration: '30s',
};

const client = new grpc.Client();
client.load(['../'], 'data.proto');

export default function () {
  // ✅ conectar SOLO la primera vez por VU
  if (!__ITER) {
    client.connect('localhost:5002', { plaintext: true });
  }

  const response = client.invoke('Data.DataService/GetData', {}, {
    metadata: { size: 'large' },
  });

  check(response, {
    'status OK': (r) => r && r.status === grpc.StatusOK,
  });
}