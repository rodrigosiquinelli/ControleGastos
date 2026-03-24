import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import api from '../services/api';
import { usePagination } from '../hooks/usePagination';

// Componente de relatório que lista o balanço financeiro por pessoa.
export function ListaPessoasRelatorio() {
  const [pessoas, setPessoas] = useState<any[]>([]);
  const [transacoes, setTransacoes] = useState<any[]>([]);
  const navigate = useNavigate();

  const { currentItems, currentPage, totalPages, goToNext, goToPrev } = usePagination(pessoas, 10);

  // Inicializa o carregamento de dados buscando pessoas e transações em paralelo.
  useEffect(() => {
    async function carregarDados() {
      try {
        const [resP, resT] = await Promise.all([
          api.get('/Pessoas'),
          api.get('/Transacoes')
        ]);
        setPessoas(resP.data || []);
        setTransacoes(resT.data || []);
      } catch (err) {
        console.error("Erro ao carregar dados do relatório:", err);
      }
    }
    carregarDados();
  }, []);

  // Filtra as transações de uma pessoa específica e calcula os totais de receitas, despesas e saldo líquido.
  const calcularTotaisPessoa = (pessoaId: string) => {
    const tPessoa = transacoes.filter(t => t.pessoaId === pessoaId);
    const receitas = tPessoa.filter(t => t.tipo === 2).reduce((acc, t) => acc + Number(t.valor), 0);
    const despesas = tPessoa.filter(t => t.tipo === 1).reduce((acc, t) => acc + Number(t.valor), 0);
    return { receitas, despesas, saldo: receitas - despesas };
  };

  // Somatório financeiro de todas as pessoas cadastradas no sistema.
  const totalGeralReceitas = transacoes.filter(t => t.tipo === 2).reduce((acc, t) => acc + Number(t.valor), 0);
  const totalGeralDespesas = transacoes.filter(t => t.tipo === 1).reduce((acc, t) => acc + Number(t.valor), 0);
  const saldoLiquidoGeral = totalGeralReceitas - totalGeralDespesas;

  return (
    <div className="max-w-5xl mx-auto space-y-8 text-left animate-in fade-in duration-500">
      <div className="flex items-center justify-between">
        <div className="flex items-center gap-4">
          <button onClick={() => navigate('/totais')} className="bg-white p-3 rounded-xl shadow-sm hover:bg-gray-50 font-bold border border-gray-100">←</button>
          <h1 className="text-2xl font-black text-gray-800 uppercase tracking-tighter">Totais por Pessoa</h1>
        </div>
      </div>

      {/* Exibição das pessoas com seus respectivos resumos financeiros. */}
      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        {currentItems.map(p => {
          const { receitas, despesas, saldo } = calcularTotaisPessoa(p.id);
          return (
            <div 
              key={p.id} 
              onClick={() => navigate(`/relatorios/pessoa/${p.id}`)}
              className="bg-white p-6 rounded-[2rem] border border-gray-100 shadow-sm hover:shadow-xl hover:border-blue-400 cursor-pointer transition-all flex flex-col gap-4 group"
            >
              <div className="flex justify-between items-start">
                <div>
                  <p className="font-black text-gray-800 uppercase tracking-tight group-hover:text-blue-600 transition-colors">{p.nome}</p>
                  <p className="text-[10px] text-gray-400 font-black uppercase tracking-widest">{p.idade} Anos</p>
                </div>
                <span className="text-blue-500 font-black text-xs opacity-0 group-hover:opacity-100 transition-all">DETALHES →</span>
              </div>

              {/* Subtotal financeiro por pessoa. */}
              <div className="grid grid-cols-3 gap-2 pt-2 border-t border-gray-50">
                <div className="text-center">
                  <p className="text-[8px] font-black text-gray-400 uppercase">Receitas</p>
                  <p className="text-xs font-bold text-green-600">R$ {receitas.toFixed(2)}</p>
                </div>
                <div className="text-center border-x border-gray-50">
                  <p className="text-[8px] font-black text-gray-400 uppercase">Despesas</p>
                  <p className="text-xs font-bold text-red-600">R$ {despesas.toFixed(2)}</p>
                </div>
                <div className="text-center">
                  <p className="text-[8px] font-black text-gray-400 uppercase">Saldo</p>
                  <p className={`text-xs font-black ${saldo >= 0 ? 'text-blue-600' : 'text-red-600'}`}>R$ {saldo.toFixed(2)}</p>
                </div>
              </div>
            </div>
          );
        })}
      </div>
      
      {/* Controles de Navegação da Paginação */}
      {totalPages > 1 && (
        <div className="flex justify-between items-center bg-gray-50/50 p-4 rounded-2xl border border-gray-100">
           <p className="text-[10px] font-black text-gray-400 uppercase">Página {currentPage} de {totalPages}</p>
           <div className="flex gap-2">
              <button onClick={goToPrev} disabled={currentPage === 1} className="px-4 py-2 bg-white rounded-lg border text-xs font-black disabled:opacity-30">Anterior</button>
              <button onClick={goToNext} disabled={currentPage === totalPages} className="px-4 py-2 bg-white rounded-lg border text-xs font-black disabled:opacity-30">Próxima</button>
           </div>
        </div>
      )}

      {/* Painel que apresenta o balanço total de todas as transações do sistema. */}
      <div className="mt-12 bg-gray-900 p-8 rounded-[2.5rem] shadow-2xl shadow-blue-900/20 text-white relative overflow-hidden">
        <div className="absolute top-0 right-0 p-8 opacity-10 text-9xl font-black">Σ</div>
        <h2 className="text-xs font-black uppercase tracking-[0.3em] mb-6 text-blue-400">Resumo Geral do Sistema</h2>
        
        <div className="grid grid-cols-1 md:grid-cols-3 gap-8 relative z-10">
          <div>
            <p className="text-gray-400 text-[10px] font-black uppercase tracking-widest mb-1">Total de Receitas</p>
            <p className="text-3xl font-black text-green-400">R$ {totalGeralReceitas.toFixed(2)}</p>
          </div>
          <div className="md:border-x md:border-gray-800 md:px-8">
            <p className="text-gray-400 text-[10px] font-black uppercase tracking-widest mb-1">Total de Despesas</p>
            <p className="text-3xl font-black text-red-400">R$ {totalGeralDespesas.toFixed(2)}</p>
          </div>
          <div>
            <p className="text-gray-400 text-[10px] font-black uppercase tracking-widest mb-1">Saldo Líquido Geral</p>
            <p className={`text-3xl font-black ${saldoLiquidoGeral >= 0 ? 'text-blue-400' : 'text-red-400'}`}>
              R$ {saldoLiquidoGeral.toFixed(2)}
            </p>
          </div>
        </div>
      </div>
      
      {/* Estado vazio para quando não há registros retornados pela API */}
      {pessoas.length === 0 && (
        <p className="p-20 text-center text-gray-400 italic font-medium">Nenhuma pessoa cadastrada para exibir totais.</p>
      )}
    </div>
  );
}
