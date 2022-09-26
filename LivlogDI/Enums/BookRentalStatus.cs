namespace LivlogDI.Enums
{
    public enum BookRentalStatus : byte
    {
        /// <summary>
        /// O livro está na posse do cliente.
        /// </summary>
        Active = 1,

        /// <summary>
        /// O cliente está na fila de espera para alugar o livro.
        /// </summary>
        WaitingQueue = 2,

        /// <summary>
        /// O cliente devolveu o livro. Pode haver ou não uma multa.
        /// </summary>
        Returned = 3
    }
}
