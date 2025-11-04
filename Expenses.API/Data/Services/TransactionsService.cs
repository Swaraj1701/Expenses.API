using Expenses.API.DTOs;
using Expenses.API.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;

namespace Expenses.API.Data.Services
{
    public interface ITransactionsService
    {
        Task<List<Transaction>> GetAll(int userId);
        Task<Transaction?> GetById(int id);
        Task<Transaction> Create(PostTransactionDto transaction, int userId);
        Task<Transaction?> Update(int id, PutTransactionDto transaction);
        Task Delete(int id);
    }
    public class TransactionsService : ITransactionsService
    {
        private readonly AppDbContext _context;
        public TransactionsService(AppDbContext context )
        {
            _context = context;
        }
        public async Task<Transaction> Create(PostTransactionDto transaction, int userId)
        {
            var newTransaction = new Transaction()
            {
                Amount = transaction.Amount,
                Type = transaction.Type,
                Category = transaction.Category,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                UserId = userId
            };
            await _context.Transactions.AddAsync(newTransaction);
            await _context.SaveChangesAsync();
            return newTransaction;
        }

        public async Task Delete(int id)
        {
            var transaction = await _context.Transactions.FirstOrDefaultAsync(x => x.Id == id);
            if (transaction != null)
            {
                var ret = _context.Transactions.Remove(transaction);
                
                await _context.SaveChangesAsync();
                
            }
        }

        public async Task<List<Transaction>> GetAll(int userId)
        {
            var allTransactions = await _context.Transactions.Where(x=> x.UserId == userId).ToListAsync();
            return allTransactions;
        }

        public async Task<Transaction?> GetById(int id)
        {
            var transaction = await _context.Transactions.FirstOrDefaultAsync(x => x.Id == id);
            return transaction;

        }

        public async Task<Transaction?> Update(int id, PutTransactionDto transaction)
        {
            var transactionData = await _context.Transactions.FirstOrDefaultAsync(x => x.Id == id);
            if (transactionData != null)
            {
                transactionData.Type = transaction.Type;
                transactionData.Category = transaction.Category;
                transactionData.Amount = transaction.Amount;
                transactionData.UpdatedAt = DateTime.UtcNow;
                _context.Transactions.Update(transactionData);
                await _context.SaveChangesAsync();
            }

            return transactionData;
        }
    }
}
