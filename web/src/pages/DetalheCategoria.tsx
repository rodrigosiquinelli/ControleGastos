import { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import api from '../services/api';
import { usePagination } from '../hooks/usePagination'; // Importando o hook

export function DetalheCategoria() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [categoria, setCategoria] = useState<any>(null);
  const [transacoes, setTransacoes] = useState<any[]>([]);

  // Inicializando a paginação com a lista de transações
  const { currentItems, currentPage, totalPages, goToNext, goToPrev } = usePagination(transacoes, 10);

  useEffect(() => {
    async function carregar() {
      try {
        const [resC, resT] = await Promise.all([
          api.get(`/Categorias/${id}`),
          api.get(`/Transacoes`)
        ]);
        
        setCategoria(resC.data);
        
        const filtradas = (resT.data || [])
          .filter((t: any) => t.categoriaId === id)
          .map((t: any) => ({
            ...t,
            valor: Number(t.valor)
          }));
          
        setTransacoes(filtradas);
      } catch (err) {
        console.error("Erro ao carregar categoria:", err);
      }
    }
    carregar();
  }, [id]);

  if (!categoria) return <div className="p-20 text-center font-bold text-gray-400">Carregando categoria...</div>;

  // IMPORTANTE: Os cálculos de totais devem usar o array COMPLETO (transacoes)
  const totalReceita = transacoes.filter(t => t.tipo === 2).reduce((acc, t) => acc + t.valor, 0);
  const totalDespesa = transacoes.filter(t => t.tipo === 1).reduce((acc, t) => acc + t.valor, 0);
  const saldo = totalReceita - totalDespesa;

  return (
    <div className="max-w-4xl mx-auto space-y-6 text-left animate-in fade-in">
      <button onClick={() => navigate(-1)} className="font-bold text-green-600 hover:underline">← Voltar</button>

      {/* Header com Totais Reais (Independente da página atual) */}
      <div className="bg-white p-10 rounded-[2.5rem] shadow-xl shadow-gray-200/50 border border-gray-100">
        <div className="flex flex-col md:flex-row justify-between items-center gap-8">
          <div>
            <h1 className="text-4xl font-black text-gray-900 uppercase tracking-tighter">{categoria.descricao}</h1>
            <p className="text-gray-400 font-bold mt-1 uppercase text-xs">Finalidade: {categoria.finalidade || 'Não informada'}</p>
          </div>
          
          <div className="card-detalhes  p-6 rounded-[2rem] flex gap-8 border border-gray-100">
            <div className="text-center">
              <p className="text-[10px] font-black text-gray-400 uppercase">Entradas</p>
              <p className="text-xl font-black text-green-600">R$ {totalReceita.toFixed(2)}</p>
            </div>
            <div className="text-center border-x px-8">
              <p className="text-[10px] font-black text-gray-400 uppercase">Saídas</p>
              <p className="text-xl font-black text-red-600">R$ {totalDespesa.toFixed(2)}</p>
            </div>
            <div className="text-center">
              <p className="text-[10px] font-black text-gray-400 uppercase">Balanço</p>
              <p className={`text-xl font-black ${saldo >= 0 ? 'text-blue-600' : 'text-red-600'}`}>
                R$ {saldo.toFixed(2)}
              </p>
            </div>
          </div>
        </div>
      </div>

      {/* Tabela de Transações Pagina (Usa currentItems) */}
      <div className="bg-white rounded-[2rem] shadow-sm border border-gray-100 overflow-hidden">
        <div className="px-8 py-4 bg-gray-50/50 border-b">
            <span className="text-[10px] font-black text-gray-400 uppercase tracking-widest">Movimentações nesta categoria</span>
        </div>
        <table className="w-full">
          <tbody className="divide-y divide-gray-50">
            {currentItems.map(t => (
              <tr key={t.id}>
                <td className="px-8 py-6">
                  <p className="font-bold text-gray-800 text-lg">{t.descricao}</p>
                  <span className="text-[10px] font-black text-gray-400 uppercase tracking-tight">Responsável: {t.pessoaNome}</span>
                </td>
                <td className={`px-8 py-6 text-right font-black text-xl ${t.tipo === 2 ? 'text-green-600' : 'text-red-600'}`}>
                  {t.tipo === 2 ? '+' : '-'} R$ {t.valor.toFixed(2)}
                </td>
              </tr>
            ))}
          </tbody>
        </table>

        {/* Controles de Paginação */}
        {totalPages > 1 && (
          <div className="p-6 bg-gray-50/30 border-t flex items-center justify-between">
            <span className="text-xs font-black text-gray-400 uppercase">Página {currentPage} de {totalPages}</span>
            <div className="flex gap-2">
              <button 
                onClick={goToPrev}
                disabled={currentPage === 1}
                className="px-6 py-2 rounded-xl bg-white border-2 border-gray-100 font-black text-gray-600 hover:bg-gray-50 disabled:opacity-30 transition-all shadow-sm"
              >
                Anterior
              </button>
              <button 
                onClick={goToNext}
                disabled={currentPage === totalPages}
                className="px-6 py-2 rounded-xl bg-white border-2 border-gray-100 font-black text-gray-600 hover:bg-gray-50 disabled:opacity-30 transition-all shadow-sm"
              >
                Próxima
              </button>
            </div>
          </div>
        )}

        {transacoes.length === 0 && <p className="p-20 text-center text-gray-400 italic">Nenhum registro para esta categoria.</p>}
      </div>
    </div>
  );
}