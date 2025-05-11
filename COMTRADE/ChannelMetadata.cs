namespace COMTRADE
{
    /// <summary>
    /// Представляет собой минимальный набор метаданных для конфигурационного файла формата COMTRADE.
    /// </summary>
    public struct ChannelMetadata
    {
        /// <summary>
        /// Название канала.
        /// </summary>
        public string Name;

        /// <summary>
        /// Тип сигнала канала.
        /// </summary>
        public SignalType SignalType;

        /// <summary>
        /// Определяет, является ли канал дискретным.
        /// </summary>
        /// <remarks>
        /// Канал считается аналоговым, если поле имеет значение <c>false</c>.
        /// </remarks>
        public bool IsDigital;

        /// <summary>
        /// Определяет единицу измерения, используемую каналом.
        /// </summary>
        /// <remarks>
        /// Оставьте <c>null</c> для параметров по умолчанию.
        /// </remarks>
        public string? Units;

        /// <summary>
        /// Определяет участок цепи, используемый каналом.
        /// </summary>
        /// <remarks>
        /// Оставьте <c>null</c>, чтобы не указывать.
        /// </remarks>
        public string? CircuitComponent;
    }

    /// <summary>
    /// Определяет класс сравнения сортировки свойств метаданных.
    /// </summary>
    public class ChannelMetadataSorter : IComparer<ChannelMetadata>
    {
        /// <summary>
        /// Сравнивает одну запись метаданных с другой.
        /// </summary>
        /// <param name="left">Левая запись метаданных для сравнения.</param>
        /// <param name="right">Правая запись метаданных для сравнения.</param>
        /// <returns>Сравнение порядка сортировки записей метаданных.</returns>
        public int Compare(ChannelMetadata left, ChannelMetadata right)
        {
            // Убедитесь, что дискретные значения (в первую очередь флаги состояний) находятся позади аналоговых значений.
            // Все значения, отсутствующие в словаре, будут возвращать 0, таким образом сортируясь выше.
            s_sortOrder.TryGetValue(left.SignalType, out int leftIndex);
            s_sortOrder.TryGetValue(right.SignalType, out int rightIndex);

            return leftIndex.CompareTo(rightIndex);
        }

        /// <summary>
        /// Экземпляр сортировщика записей метаданных по умолчанию.
        /// </summary>
        public static readonly ChannelMetadataSorter Default;

        private static readonly Dictionary<SignalType, int> s_sortOrder;

        static ChannelMetadataSorter()
        {
            // Определите правильный порядок сортировки для ключевых типов сигналов. Флаги состояний являются типами
            // дискретных полей, но они хранятся как 32-битные значения с абстрактным набором флагов (старший порядок),
            // также как и исходные флаги (младший порядок), поскольку они больше 16-бит, они исторически определяются
            // как аналоговые значения. Но даже при этом, их нужно сортировать как дискретные значения.
            s_sortOrder = new Dictionary<SignalType, int>
            {
                { SignalType.FLAG, 1 }, // Флаги состояний
                { SignalType.DIGI, 2 }, // Дискретные значения
                { SignalType.QUAL, 3 }  // Флаги качества
            };

            Default = new ChannelMetadataSorter();
        }
    }
}
