import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import api from '../services/api';
import { usePagination } from '../hooks/usePagination';

export function ListaCategoriasRelatorio() {
  const [categorias, setCategorias] = useState<any[]>([]);
  const [transacoes, setTransacoes] = useState<any[]>([]);
  const navigate = useNavigate();

  const { currentItems, currentPage, totalPages, goToNext, goToPrev } = usePagination(categorias, 10);

  useEffect(() => {
    async function carregarDados() {
      try {
        const [resC, resT] = await Promise.all([
          api.get('/Categorias'),
          api.get('/Transacoes')
        ]);
        setCategorias(resC.data || []);
        setTransacoes(resT.data || []);
      } catch (err) {
        console.error("Erro ao carregar dados:", err);
      }
    }
    carregarDados();
  }, []);

  // Função para calcular totais de uma categoria específica
  const calcularTotaisCategoria = (categoriaId: string) => {
    const tCat = transacoes.filter(t => t.categoriaId === categoriaId);
    const receitas = tCat.filter(t => t.tipo === 2).reduce((acc, t) => acc + Number(t.valor), 0);
    const despesas = tCat.filter(t => t.tipo === 1).reduce((acc, t) => acc + Number(t.valor), 0);
    return { receitas, despesas, saldo: receitas - despesas };
  };

  // CÁLCULO DO TOTAL GERAL DE CATEGORIAS
  const totalGeralReceitas = transacoes.filter(t => t.tipo === 2).reduce((acc, t) => acc + Number(t.valor), 0);
  const totalGeralDespesas = transacoes.filter(t => t.tipo === 1).reduce((acc, t) => acc + Number(t.valor), 0);
  const saldoLiquidoGeral = totalGeralReceitas - totalGeralDespesas;

  return (
    <div className="max-w-5xl mx-auto space-y-8 text-left animate-in fade-in duration-500">
      <div className="flex items-center gap-4">
        <button onClick={() => navigate('/totais')} className="bg-white p-3 rounded-xl shadow-sm hover:bg-gray-50 border border-gray-100 font-bold">←</button>
        <h1 className="text-2xl font-black text-gray-800 uppercase tracking-tighter">Totais por Categoria</h1>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
        {currentItems.map(c => {
          const fin = Number(c.finalidade);
          const labelFinalidade = fin === 2 ? 'Receita' : fin === 1 ? 'Despesa' : 'Ambas';
          const { receitas, despesas, saldo } = calcularTotaisCategoria(c.id);

          return (
            <div 
              key={c.id} 
              onClick={() => navigate(`/relatorios/categoria/${c.id}`)}
              className="bg-white p-6 rounded-[2rem] border border-gray-100 shadow-sm hover:shadow-xl hover:border-green-400 cursor-pointer transition-all flex flex-col gap-4 group"
            >
              <div className="flex justify-between items-start">
                <div>
                  <p className="font-black text-gray-800 uppercase text-sm tracking-tight group-hover:text-green-600 transition-colors">
                    {c.descricao}
                  </p>
                  <div className="flex items-center gap-2 mt-1">
                    <span className={`w-1.5 h-1.5 rounded-full ${fin === 2 ? 'bg-green-500' : fin === 1 ? 'bg-red-500' : 'bg-blue-500'}`}></span>
                    <p className="text-[10px] text-gray-400 font-black uppercase tracking-widest">{labelFinalidade}</p>
                  </div>
                </div>
                <span className="text-green-500 font-black text-xs opacity-0 group-hover:opacity-100 transition-all">VER →</span>
              </div>

              {/* Mini Resumo por Categoria */}
              <div className="grid grid-cols-3 gap-2 pt-2 border-t border-gray-50">
                <div className="text-center">
                  <p className="text-[8px] font-black text-gray-400 uppercase">Rec.</p>
                  <p className="text-[10px] font-bold text-green-600">R$ {receitas.toFixed(2)}</p>
                </div>
                <div className="text-center border-x border-gray-50">
                  <p className="text-[8px] font-black text-gray-400 uppercase">Desp.</p>
                  <p className="text-[10px] font-bold text-red-600">R$ {despesas.toFixed(2)}</p>
                </div>
                <div className="text-center">
                  <p className="text-[8px] font-black text-gray-400 uppercase">Saldo</p>
                  <p className={`text-[10px] font-black ${saldo >= 0 ? 'text-blue-600' : 'text-red-600'}`}>R$ {saldo.toFixed(2)}</p>
                </div>
              </div>
            </div>
          );
        })}
      </div>

      {/* Paginação */}
      {totalPages > 1 && (
        <div className="flex justify-between items-center bg-gray-50/50 p-4 rounded-2xl border border-gray-100">
           <p className="text-[10px] font-black text-gray-400 uppercase">Página {currentPage} de {totalPages}</p>
           <div className="flex gap-2">
              <button onClick={goToPrev} disabled={currentPage === 1} className="px-4 py-2 bg-white rounded-lg border text-xs font-black disabled:opacity-30">Anterior</button>
              <button onClick={goToNext} disabled={currentPage === totalPages} className="px-4 py-2 bg-white rounded-lg border text-xs font-black disabled:opacity-30">Próxima</button>
           </div>
        </div>
      )}

      {/* SEÇÃO DE TOTAL GERAL DE CATEGORIAS */}
      <div className="mt-12 bg-gray-900 p-8 rounded-[2.5rem] shadow-2xl shadow-green-900/20 text-white relative overflow-hidden">
        <div className="absolute top-0 right-0 p-8 opacity-10 text-9xl font-black text-green-500">📊</div>
        <h2 className="text-xs font-black uppercase tracking-[0.3em] mb-6 text-green-400">Balanço Geral por Categorias</h2>
        
        <div className="grid grid-cols-1 md:grid-cols-3 gap-8 relative z-10">
          <div>
            <p className="text-gray-400 text-[10px] font-black uppercase tracking-widest mb-1">Receita Total</p>
            <p className="text-3xl font-black text-green-400">R$ {totalGeralReceitas.toFixed(2)}</p>
          </div>
          <div className="md:border-x md:border-gray-800 md:px-8">
            <p className="text-gray-400 text-[10px] font-black uppercase tracking-widest mb-1">Despesa Total</p>
            <p className="text-3xl font-black text-red-400">R$ {totalGeralDespesas.toFixed(2)}</p>
          </div>
          <div>
            <p className="text-gray-400 text-[10px] font-black uppercase tracking-widest mb-1">Resultado Líquido</p>
            <p className={`text-3xl font-black ${saldoLiquidoGeral >= 0 ? 'text-blue-400' : 'text-red-400'}`}>
              R$ {saldoLiquidoGeral.toFixed(2)}
            </p>
          </div>
        </div>
      </div>

      {categorias.length === 0 && (
        <p className="p-20 text-center text-gray-400 italic">Nenhuma categoria registrada.</p>
      )}
    </div>
  );
}