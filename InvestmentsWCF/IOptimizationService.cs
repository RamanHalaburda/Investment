using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace InvestmentsWCF
{
    // ПРИМЕЧАНИЕ. Команду "Переименовать" в меню "Рефакторинг" можно использовать для одновременного изменения имени интерфейса "IOptimizationService" в коде и файле конфигурации.
    [ServiceContract]
    public interface IOptimizationService
    {
        [OperationContract]
        double[] DoSimplex(double _sum, double _ratio, double _divA, double _divB, double _limA, double _limB);

        [OperationContract]
        double[] DoBionic(int N, int G, double _sum, double _ratio, double _divA, double _divB, double _limA, double _limB);
    }
}
