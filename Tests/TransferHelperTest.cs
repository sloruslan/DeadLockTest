using DeadLockTest;

namespace Tests
{
    public class TransferHelperTest
    {
        [Fact]
        public async Task DeadlockTest()
        {
            var ivan = new Account() { AccountNumber = 1, Amount = 1000, ownerName = "Ivan" };
            var olga = new Account() { AccountNumber = 2, Amount = 500, ownerName = "Olga" };

            TransferHelper helper = new TransferHelper();

            var task1 = Task.Run(() => helper.TransferLock(ivan, olga, 100));
            var task2 = Task.Run(() => helper.TransferLock(olga, ivan, 10));

            await Task.WhenAll(task1, task2);

            Assert.Equal(910, ivan.Amount);
            Assert.Equal(590, olga.Amount);

        }

        [Fact]
        public async Task NormalTest()
        {
            var ivan = new Account() { AccountNumber = 1, Amount = 1000, ownerName = "Ivan" };
            var olga = new Account() { AccountNumber = 2, Amount = 500, ownerName = "Olga" };

            TransferHelper helper = new TransferHelper();

            var task1 = Task.Run(() => helper.TransferSuccess(ivan, olga, 100));
            var task2 = Task.Run(() => helper.TransferSuccess(olga, ivan, 10));

            await Task.WhenAll(task1, task2);
            Assert.Equal(910, ivan.Amount);
            Assert.Equal(590, olga.Amount);

        }

    }
}
