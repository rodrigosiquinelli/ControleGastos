import axios from 'axios';

// Aqui criamos a conexão. 
const api = axios.create({
  baseURL: 'http://localhost:5000/api', // Ajuste a porta se a sua for diferente
});

export default api;
