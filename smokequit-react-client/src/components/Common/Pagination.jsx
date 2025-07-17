import React from 'react';

const Pagination = ({ 
  currentPage, 
  totalPages, 
  totalItems, 
  pageSize, 
  onPageChange,
  loading = false 
}) => {
  const startIndex = (currentPage - 1) * pageSize + 1;
  const endIndex = Math.min(currentPage * pageSize, totalItems);

  const getPageNumbers = () => {
    const pages = [];
    const maxVisible = 5;
    
    let start = Math.max(1, currentPage - Math.floor(maxVisible / 2));
    let end = Math.min(totalPages, start + maxVisible - 1);
    
    if (end - start + 1 < maxVisible) {
      start = Math.max(1, end - maxVisible + 1);
    }

    for (let i = start; i <= end; i++) {
      pages.push(i);
    }
    
    return pages;
  };

  if (totalPages <= 1) return null;

  return (
    <div className="d-flex justify-content-between align-items-center mt-4">
      <div className="text-muted">
        Showing {startIndex} to {endIndex} of {totalItems} results
      </div>
      
      <nav aria-label="Page navigation">
        <ul className="pagination mb-0">
          {/* First Page */}
          <li className={`page-item ${currentPage === 1 || loading ? 'disabled' : ''}`}>
            <button 
              className="page-link" 
              onClick={() => onPageChange(1)}
              disabled={currentPage === 1 || loading}
              aria-label="First"
            >
              <i className="bi bi-chevron-double-left"></i>
            </button>
          </li>

          {/* Previous Page */}
          <li className={`page-item ${currentPage === 1 || loading ? 'disabled' : ''}`}>
            <button 
              className="page-link" 
              onClick={() => onPageChange(currentPage - 1)}
              disabled={currentPage === 1 || loading}
              aria-label="Previous"
            >
              <i className="bi bi-chevron-left"></i>
            </button>
          </li>

          {/* Page Numbers */}
          {getPageNumbers().map(page => (
            <li key={page} className={`page-item ${page === currentPage ? 'active' : ''} ${loading ? 'disabled' : ''}`}>
              <button 
                className="page-link" 
                onClick={() => onPageChange(page)}
                disabled={loading}
              >
                {page}
              </button>
            </li>
          ))}

          {/* Next Page */}
          <li className={`page-item ${currentPage === totalPages || loading ? 'disabled' : ''}`}>
            <button 
              className="page-link" 
              onClick={() => onPageChange(currentPage + 1)}
              disabled={currentPage === totalPages || loading}
              aria-label="Next"
            >
              <i className="bi bi-chevron-right"></i>
            </button>
          </li>

          {/* Last Page */}
          <li className={`page-item ${currentPage === totalPages || loading ? 'disabled' : ''}`}>
            <button 
              className="page-link" 
              onClick={() => onPageChange(totalPages)}
              disabled={currentPage === totalPages || loading}
              aria-label="Last"
            >
              <i className="bi bi-chevron-double-right"></i>
            </button>
          </li>
        </ul>
      </nav>
    </div>
  );
};

export default Pagination;