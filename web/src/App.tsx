import { BrowserRouter as Router, Routes, Route, Link } from 'react-router-dom';
import { Pessoas } from './pages/Pessoas';
import { Categorias } from './pages/Categorias';
import { Transacoes } from './pages/Transacoes';
import { Totais } from './pages/Totais';
import { ListaPessoasRelatorio } from './pages/ListaPessoasRelatorio';
import { ListaCategoriasRelatorio } from './pages/ListaCategoriasRelatorio';
import { DetalhePessoa } from './pages/DetalhePessoa';
import { DetalheCategoria } from './pages/DetalheCategoria';

function App() {
  return (
    <Router>
      {/* Removido o 'bg-gray-50' para que a cor do index.css (body) apareça */}
      <div className="min-h-screen font-sans">
        {/* Menu de Navegação */}
        <nav className="bg-white shadow-sm border-b border-gray-100 px-4 py-4 mb-8">
          <div className="max-w-6xl mx-auto flex flex-col md:flex-row items-center justify-between gap-4">
            <Link to="/" className="text-2xl font-black text-blue-600 flex items-center gap-2">
              <span className="bg-blue-600 text-white p-2 rounded-lg text-sm">CG</span>
              Controle Residencial
            </Link>
            
            <div className="flex items-center gap-6 font-bold text-gray-500 text-sm uppercase tracking-tighter">
              <Link to="/pessoas" className="hover:text-blue-600 transition-colors">Pessoas</Link>
              <Link to="/categorias" className="hover:text-blue-600 transition-colors">Categorias</Link>
              <Link to="/transacoes" className="hover:text-blue-600 transition-colors">Transações</Link>
              <Link to="/totais" className="bg-gray-900 text-white px-4 py-2 rounded-xl hover:bg-blue-600 transition-all flex items-center gap-2">
                📊 Relatórios
              </Link>
            </div>
          </div>
        </nav>

        {/* Área de Conteúdo */}
        <main className="max-w-6xl mx-auto px-4 pb-20">
          <Routes>
            <Route path="/pessoas" element={<Pessoas />} />
            <Route path="/categorias" element={<Categorias />} />
            <Route path="/transacoes" element={<Transacoes />} />
            
            <Route path="/totais" element={<Totais />} />
            <Route path="/relatorios/lista-pessoas" element={<ListaPessoasRelatorio />} />
            <Route path="/relatorios/pessoa/:id" element={<DetalhePessoa />} />
            <Route path="/relatorios/lista-categorias" element={<ListaCategoriasRelatorio />} />
            <Route path="/relatorios/categoria/:id" element={<DetalheCategoria />} />
            
            <Route path="/" element={
              <div className="text-center py-32 bg-white rounded-[3rem] shadow-xl shadow-gray-200/50 border border-gray-100">
                <h1 className="text-5xl font-black text-gray-900 mb-4 tracking-tight">
                  Gestão Financeira <span className="text-blue-600">Simplificada</span>.
                </h1>
                <p className="text-gray-400 text-lg max-w-md mx-auto">
                  Monitore seus gastos residenciais e de sua família em um só lugar com relatórios detalhados.
                </p>
                <div className="mt-10 flex justify-center gap-4">
                  <Link to="/transacoes" className="bg-blue-600 text-white font-black px-8 py-4 rounded-2xl shadow-lg shadow-blue-200 hover:scale-105 transition-transform">
                    Lançar Agora
                  </Link>
                </div>
              </div>
            } />
          </Routes>
        </main>
      </div>
    </Router>
  );
}

export default App;