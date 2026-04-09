import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import api from '../services/api';
import { usePagination } from '../hooks/usePagination';

interface Pessoa {
  id: string;
  nome: string;
  idade: number;
  dataNascimento?: string;
}

// Componente de página para gerenciamento de pessoas, permitindo listagem, cadastro, edição e exclusão.
export function Pessoas() {
  const [pessoas, setPessoas] = useState<Pessoa[]>([]);
  const [nome, setNome] = useState('');
  const [dataNascimento, setDataNascimento] = useState('');
  const [carregando, setCarregando] = useState(false);
  const [editandoId, setEditandoId] = useState<string | null>(null); 
  
  const navigate = useNavigate();
  const { currentItems, currentPage, totalPages, goToNext, goToPrev } = usePagination(pessoas, 10);

  // Carrega a lista inicial de pessoas assim que o componente é montado na tela.
  useEffect(() => { carregarPessoas(); }, []);

  // Busca os dados da API e atualiza o estado local de pessoas.
  async function carregarPessoas() {
    try {
      const res = await api.get('/Pessoas');
      setPessoas(res.data || []);
    } catch (err) { console.error(err); }
  }

  // Função auxiliar para calcular a idade com base na data de nascimento selecionada no formulário.
  function calcularIdade(data: string): number {
    const hoje = new Date();
    const nascimento = new Date(data);
    let idade = hoje.getFullYear() - nascimento.getFullYear();
    const mes = hoje.getMonth() - nascimento.getMonth();
    if (mes < 0 || (mes === 0 && hoje.getDate() < nascimento.getDate())) idade--;
    return idade;
  }

  // Preenche o formulário com os dados da pessoa selecionada e rola a tela para o topo para edição.
  function prepararEdicao(p: Pessoa, e: React.MouseEvent) {
    e.stopPropagation();
    setEditandoId(p.id);
    setNome(p.nome);
    if (p.dataNascimento) {
      setDataNascimento(p.dataNascimento.split('T')[0]);
    }
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  // Limpa os campos do formulário e encerra o modo de edição.
  function cancelarEdicao() {
    setEditandoId(null);
    setNome('');
    setDataNascimento('');
  }

  // Envia os dados para a API, alternando entre criação ou atualização conforme o estado.
  async function salvar(e: React.FormEvent) {
    e.preventDefault();
    if (nome.trim().length < 3) return alert("Digite o nome completo.");
    if (!dataNascimento) return alert("Selecione a data.");

    const idadeCalculada = calcularIdade(dataNascimento);

    try {
      setCarregando(true);
      const dataFormatadaUniversal = new Date(dataNascimento + "T00:00:00Z").toISOString();
      
      const payload = { nome, idade: idadeCalculada, dataNascimento: dataFormatadaUniversal };

      if (editandoId) {
        await api.put(`/Pessoas/${editandoId}`, { ...payload, id: editandoId });
        alert("✅ Atualizado com sucesso!");
      } else {
        await api.post('/Pessoas', payload);
        alert("✅ Cadastrado com sucesso!");
      }
      
      cancelarEdicao();
      carregarPessoas();
    } catch (error: any) {
      alert("❌ Erro ao salvar dados.");
    } finally {
      setCarregando(false);
    }
  }

  // Remove um registro de pessoa após a confirmação do usuário e atualiza a lista.
  async function excluir(id: string, e: React.MouseEvent) {
    e.stopPropagation(); 
    if (window.confirm("Tem certeza? Isso apagará todas as transações desta pessoa.")) {
      try {
        await api.delete(`/Pessoas/${id}`);
        if (editandoId === id) cancelarEdicao();
        carregarPessoas();
      } catch { alert("Erro ao excluir."); }
    }
  }

  return (
    <div className="max-w-5xl mx-auto space-y-8 animate-in fade-in duration-500">
      <div className="text-left border-l-4 border-blue-600 pl-4">
        <h1 className="text-3xl font-black text-gray-800">
          {editandoId ? 'Editando Pessoa' : 'Pessoas'}
        </h1>
        <p className="text-gray-500">
          {editandoId ? `Alterando dados de ${nome}` : 'Gerencie os moradores e seus dados'}
        </p>
      </div>

      {/* Formulário de Cadastro e Edição */}
      <form onSubmit={salvar} className={`bg-white p-8 rounded-3xl shadow-xl border-2 transition-all ${editandoId ? 'border-orange-400' : 'border-transparent'} flex flex-col md:flex-row gap-6 items-end`}>
        <div className="flex-1 w-full text-left">
        <label htmlFor="nome" className="text-sm font-bold text-gray-600 ml-1">Nome Completo</label>
        <input 
          id="nome" 
          className="w-full mt-1 border-2 border-gray-100 rounded-2xl p-4 focus:border-blue-500 outline-none transition-all bg-gray-50/50" 
          value={nome} 
          onChange={e => setNome(e.target.value)} 
          required 
        />
      </div>
        <div className="w-full md:w-64 text-left">
        <label htmlFor="dataNascimento" className="text-sm font-bold text-gray-600 ml-1">Data de Nascimento</label>
        <input 
          id="dataNascimento" 
          type="date" 
          className="w-full mt-1 border-2 border-gray-100 rounded-2xl p-4 focus:border-blue-500 outline-none bg-gray-50/50" 
          value={dataNascimento} 
          onChange={e => setDataNascimento(e.target.value)} 
          required 
        />
      </div>
        
        <div className="flex gap-2 w-full md:w-auto">
          {editandoId && (
            <button type="button" onClick={cancelarEdicao} className="bg-gray-100 text-gray-600 font-bold py-5 px-6 rounded-2xl hover:bg-gray-200 transition-all">
              CANCELAR
            </button>
          )}
          <button disabled={carregando} className={`${editandoId ? 'bg-orange-500 hover:bg-orange-600' : 'bg-blue-600 hover:bg-blue-700'} text-white font-black py-5 px-10 rounded-2xl shadow-lg transition-all disabled:opacity-50 flex-1`}>
            {carregando ? "..." : editandoId ? "SALVAR ALTERAÇÃO" : "CADASTRAR"}
          </button>
        </div>
      </form>

      {/* Grid de Cards para exibição das pessoas cadastradas */}
      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6">
        {currentItems.map(p => (
          <div 
            key={p.id} 
            onClick={() => navigate(`/relatorios/pessoa/${p.id}`)}
            className={`bg-white p-6 rounded-3xl shadow-sm border-2 flex justify-between items-center group transition-all cursor-pointer ${editandoId === p.id ? 'border-orange-400 scale-[1.02]' : 'border-gray-100 hover:border-blue-400 hover:shadow-xl'}`}
          >
            <div className="text-left">
              <p className="font-black text-gray-800 uppercase text-sm group-hover:text-blue-600 transition-colors">{p.nome}</p>
              <p className="text-gray-400 text-xs font-bold">{p.idade} ANOS</p>
            </div>
            
            <div className="flex gap-2">
              <button onClick={(e) => prepararEdicao(p, e)} className="p-3 rounded-xl bg-orange-50 text-orange-500 opacity-0 group-hover:opacity-100 hover:bg-orange-500 hover:text-white transition-all">
                ✏️
              </button>
              <button onClick={(e) => excluir(p.id, e)} className="p-3 rounded-xl bg-red-50 text-red-500 opacity-0 group-hover:opacity-100 hover:bg-red-500 hover:text-white transition-all">
                🗑️
              </button>
            </div>
          </div>
        ))}
      </div>

      {/* Controles de Navegação da Paginação */}
      {totalPages > 1 && (
        <div className="flex flex-col md:flex-row items-center justify-between gap-4 pt-4 border-t border-gray-100">
          <p className="text-sm font-bold text-gray-400 uppercase">Página {currentPage} de {totalPages}</p>
          <div className="flex gap-2">
            <button onClick={goToPrev} disabled={currentPage === 1} className="px-6 py-2 rounded-xl bg-white border-2 border-gray-100 font-black text-gray-600 hover:bg-gray-50 disabled:opacity-30 transition-all">Anterior</button>
            <button onClick={goToNext} disabled={currentPage === totalPages} className="px-6 py-2 rounded-xl bg-white border-2 border-gray-100 font-black text-gray-600 hover:bg-gray-50 disabled:opacity-30 transition-all">Próxima</button>
          </div>
        </div>
      )}

      {/* Estado vazio para quando não há registros retornados pela API */}
      {pessoas.length === 0 && (
        <div className="py-20 text-center bg-gray-50 rounded-3xl border-2 border-dashed border-gray-200">
          <p className="text-gray-400">Nenhuma pessoa cadastrada.</p>
        </div>
      )}
    </div>
  );
}
