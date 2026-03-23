import axios from 'axios';

// Aqui criamos a conexão. 
// O baseURL deve ser o endereço que aparece quando você dá "Play" no seu C#.
const api = axios.create({
  baseURL: 'http://localhost:5000/api', // Ajuste a porta se a sua for diferente
});

export default api;