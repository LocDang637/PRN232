using SmokeQuit.Repository.LocDPX.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmokeQuit.Repository.LocDPX
{
    public interface IUnitOfWork : IDisposable
    {
        SystemUserAccountRepository SystemUserAccountRepository { get; }
        ChatsLocDpxRepository ChatsLocDpxRepository { get; }
        CoachesLocDpxRepository CoachesLocDpxRepository { get; }
        int SaveChangesWithTransaction();
        Task<int> SaveChangesWithTransactionAsync();
    }

    public class UnitOfWork : IUnitOfWork
    {

        private SystemUserAccountRepository _systemUserAccountRepository;
        private ChatsLocDpxRepository _chatsLocDpxRepository;
        private CoachesLocDpxRepository _coachesLocDpxRepository;
        private readonly SmokeQuitDbContext _context;

        public UnitOfWork() => _context ??= new SmokeQuitDbContext();

        public SystemUserAccountRepository SystemUserAccountRepository
        {
            get { return _systemUserAccountRepository ??= new SystemUserAccountRepository(_context); }
        }
        public ChatsLocDpxRepository ChatsLocDpxRepository
        {
            get { return _chatsLocDpxRepository ??= new ChatsLocDpxRepository(_context); }
        }

        public CoachesLocDpxRepository CoachesLocDpxRepository
        {
            get { return _coachesLocDpxRepository ??= new CoachesLocDpxRepository(_context); }
        }

        public void Dispose() => _context.Dispose();

        public int SaveChangesWithTransaction()
        {
            int result = -1;

            //System.Data.IsolationLevel.Snapshot
            using (var dbContextTransaction = _context.Database.BeginTransaction())
            {
                try
                {

                    result = _context.SaveChanges();
                    dbContextTransaction.Commit();
                }
                catch (Exception)
                {
                    //Log Exception Handling message                      
                    result = -1;
                    dbContextTransaction.Rollback();
                }
            }

            return result;
        }

        public async Task<int> SaveChangesWithTransactionAsync()
        {
            int result = -1;

            //System.Data.IsolationLevel.Snapshot
            using (var dbContextTransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    result = await _context.SaveChangesAsync();
                    dbContextTransaction.Commit();
                }
                catch (Exception)
                {
                    //Log Exception Handling message                      
                    result = -1;
                    dbContextTransaction.Rollback();
                }
            }

            return result;
        }
    }
}