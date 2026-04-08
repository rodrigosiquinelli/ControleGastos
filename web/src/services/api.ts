import axios from 'axios';

// Aqui criamos a conexão. 
const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL
});

export default api;
