import { useEffect, useState } from 'react';
import api from '../services/api';
import { usePagination } from '../hooks/usePagination';

interface Pessoa { id: string; nome: string; idade: number; }
interface Categoria { id: string; descricao: string; finalidade: string; }
interface Transacao {
  id: string;
  descricao: string;
  valor: number;
  tipo: number;
  pessoaId: string;
  pessoaNome: string; 
  categoriaId: string;
}

export function Transacoes() {
  const [transacoes, setTransacoes] = useState<Transacao[]>([]);
  const [pessoas, setPessoas] = useState<Pessoa[]>([]);
  const [categorias, setCategorias] = useState<Categoria[]>([]);

  // Estados do formulário
  const [descricao, setDescricao] = useState('');
  const [valor, setValor] = useState('');
  const [tipo, setTipo] = useState('Despesa');
  const [pessoaId, setPessoaId] = useState('');
  const [categoriaId, setCategoriaId] = useState('');

  // Utilizando o Hook de Paginação
  const { currentItems, currentPage, totalPages, goToNext, goToPrev } = usePagination(transacoes, 10);

  useEffect(() => {
    carregarDadosIniciais();
  }, []);

  async function carregarDadosIniciais() {
    try {
      const [resT, resP, resC] = await Promise.all([
        api.get('/Transacoes'),
        api.get('/Pessoas'),
        api.get('/Categorias')
      ]);
      setTransacoes(resT.data || []);
      setPessoas(resP.data || []);
      setCategorias(resC.data || []);
    } catch (error) {
      console.error("Erro ao carregar dados:", error);
    }
  }

  async function salvar(e: React.FormEvent) {
  e.preventDefault();

  const valorNumerico = Number(valor);
  const tipoNumerico = tipo === 'Receita' ? 2 : 1;
  const p = pessoas.find(x => x.id === pessoaId);
  const c = categorias.find(x => x.id === categoriaId);

  // --- VALIDAÇÕES DE FRONT-END (Rápido feedback) ---

  // 1. Validação de Valor Positivo (Requisito: Valor deve ser número positivo)
  if (valorNumerico <= 0) {
    return alert("⚠️ O valor da transação deve ser maior que zero.");
  }

  // 2. Validação: Menor de 18 anos (Apenas despesas)
  if (p && p.idade < 18 && tipoNumerico === 2) {
    return alert("🚫 Menores de 18 anos não podem registrar Receitas!");
  }

  // 3. Validação: Compatibilidade de Categoria vs Tipo
  if (c) {
    const finalidade = c.finalidade; // "Receita", "Despesa" ou "Ambas"
    if (finalidade === 'Receita' && tipoNumerico === 1) {
      return alert(`🚫 A categoria "${c.descricao}" é exclusiva para RECEITAS.`);
    }
    if (finalidade === 'Despesa' && tipoNumerico === 2) {
      return alert(`🚫 A categoria "${c.descricao}" é exclusiva para DESPESAS.`);
    }
  }

  // --- ENVIO PARA API ---
  try {
    await api.post('/Transacoes', { 
      descricao, 
      valor: valorNumerico, 
      tipo: tipoNumerico, 
      pessoaId, 
      categoriaId 
    });
    
    // Limpeza e Sucesso
    setDescricao(''); 
    setValor('');
    carregarDadosIniciais(); 
    alert("✅ Lançamento realizado com sucesso!");

  } catch (error: any) {
    const apiResponse = error.response?.data;
    
    // Tenta capturar a mensagem específica do Backend
    const mensagemErro = 
      apiResponse?.Detailed || 
      apiResponse?.Message || 
      (typeof apiResponse === 'string' ? apiResponse : "Erro ao processar transação. Verifique os dados.");

    alert(`❌ Erro: ${mensagemErro}`);
    console.error("Detalhes do erro:", error);
  }
}

  return (
    <div className="max-w-6xl mx-auto space-y-8 animate-in fade-in duration-500 text-left">
      <div className="border-l-4 border-blue-600 pl-4">
        <h1 className="text-3xl font-black text-gray-800">Lançamentos</h1>
        <p className="text-gray-500">Gestão de entradas e saídas financeiras</p>
      </div>

      <form onSubmit={salvar} className="bg-white p-8 rounded-3xl shadow-xl border border-gray-100 space-y-6">
        <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
          <div className="flex flex-col gap-2">
            <label className="text-sm font-bold text-gray-700">Descrição</label>
            <input 
              className="border-2 border-gray-100 rounded-2xl p-4 focus:border-blue-500 outline-none bg-gray-50/50"
              value={descricao} onChange={e => setDescricao(e.target.value)} required 
            />
          </div>
          <div className="flex flex-col gap-2">
            <label className="text-sm font-bold text-gray-700">Valor</label>
            <input 
              type="number" className="border-2 border-gray-100 rounded-2xl p-4 focus:border-blue-500 outline-none bg-gray-50/50"
              value={valor} onChange={e => setValor(e.target.value)} step="0.01" required 
            />
          </div>
        </div>

        <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
          <select className="border-2 border-gray-100 rounded-2xl p-4 bg-gray-50/50" value={tipo} onChange={e => setTipo(e.target.value)}>
            <option value="Despesa">🔴 Despesa</option>
            <option value="Receita">🟢 Receita</option>
          </select>

          <select className="border-2 border-gray-100 rounded-2xl p-4 bg-gray-50/50" value={pessoaId} onChange={e => setPessoaId(e.target.value)} required>
            <option value="">Responsável...</option>
            {pessoas.map(p => <option key={p.id} value={p.id}>{p.nome}</option>)}
          </select>

          <select className="border-2 border-gray-100 rounded-2xl p-4 bg-gray-50/50" value={categoriaId} onChange={e => setCategoriaId(e.target.value)} required>
            <option value="">Categoria...</option>
            {categorias.map(c => <option key={c.id} value={c.id}>{c.descricao}</option>)}
          </select>
        </div>

        <button className="w-full bg-blue-600 hover:bg-blue-700 text-white font-black py-5 rounded-2xl shadow-lg transition-all active:scale-[0.98]">
          FINALIZAR LANÇAMENTO
        </button>
      </form>

      <div className="bg-white rounded-3xl shadow-sm border border-gray-100 overflow-hidden">
        <table className="w-full text-left">
          <thead className="bg-gray-50/50 border-b border-gray-100">
            <tr>
              <th className="px-8 py-5 text-xs font-bold text-gray-400 uppercase tracking-widest">Lançamento / Responsável</th>
              <th className="px-8 py-5 text-xs font-bold text-gray-400 uppercase tracking-widest text-right">Valor</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-gray-50">
            {currentItems.map(t => (
              <tr key={t.id} className="hover:bg-blue-50/20 transition-colors">
                <td className="px-8 py-6">
                  <div className="flex flex-col">
                    <span className="font-bold text-gray-800">{t.descricao}</span>
                    <div className="flex items-center gap-2 mt-1">
                       <span className={`text-[10px] font-black uppercase px-2 py-0.5 rounded ${t.tipo === 2 ? 'bg-green-100 text-green-700' : 'bg-red-100 text-red-700'}`}>
                         {t.tipo === 2 ? 'Receita' : 'Despesa'}
                       </span>
                       <span className="text-[10px] font-bold text-gray-400 uppercase tracking-tight">
                         👤 {t.pessoaNome || 'N/A'}
                       </span>
                    </div>
                  </div>
                </td>
                <td className={`px-8 py-6 text-right font-black text-lg ${t.tipo === 2 ? 'text-green-600' : 'text-red-600'}`}>
                  {t.tipo === 2 ? '+' : '-'} R$ {t.valor.toFixed(2)}
                </td>
              </tr>
            ))}
          </tbody>
        </table>

        {totalPages > 1 && (
          <div className="p-6 bg-gray-50/30 border-t flex items-center justify-between">
            <span className="text-sm text-gray-500 font-black uppercase">
              Página {currentPage} de {totalPages}
            </span>
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
        
        {transacoes.length === 0 && <p className="p-20 text-center text-gray-400 italic font-medium">Nenhum lançamento registrado.</p>}
      </div>
    </div>
  );
}