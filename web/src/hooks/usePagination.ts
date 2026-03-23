import { useState } from 'react';

export function usePagination<T>(items: T[], itemsPerPage: number = 10) {
  const [currentPage, setCurrentPage] = useState(1);

  // Cálculos
  const totalPages = Math.ceil(items.length / itemsPerPage);
  const indexOfLastItem = currentPage * itemsPerPage;
  const indexOfFirstItem = indexOfLastItem - itemsPerPage;
  const currentItems = items.slice(indexOfFirstItem, indexOfLastItem);

  const goToNext = () => setCurrentPage(prev => Math.min(prev + 1, totalPages));
  const goToPrev = () => setCurrentPage(prev => Math.max(prev - 1, 1));
  const reset = () => setCurrentPage(1);

  return {
    currentPage,
    totalPages,
    currentItems,
    goToNext,
    goToPrev,
    reset,
    itemsPerPage
  };
}