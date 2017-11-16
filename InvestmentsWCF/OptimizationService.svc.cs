using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace InvestmentsWCF
{
    // ПРИМЕЧАНИЕ. Команду "Переименовать" в меню "Рефакторинг" можно использовать для одновременного изменения имени класса "OptimizationService" в коде, SVC-файле и файле конфигурации.
    // ПРИМЕЧАНИЕ. Чтобы запустить клиент проверки WCF для тестирования службы, выберите элементы OptimizationService.svc или OptimizationService.svc.cs в обозревателе решений и начните отладку.
    public class OptimizationService : IOptimizationService
    {
        public double[] DoSimplex(double _sum, double _ratio, double _divA, double _divB, double _limA, double _limB)
        {
            Simplex simplex = new Simplex(_sum, _ratio, _divA, _divB, _limA, _limB);
            double[] res = simplex.DoSimplex();
            return res;
        }

        public double[] DoBionic(int _N, int _G, double _sum, double _ratio, double _divA, double _divB, double _limA, double _limB)
        {
            Bionic bionic = new Bionic(_sum, _ratio, _divA, _divB, _limA, _limB);
            double[] res = bionic.DoBionic(_N, _G);
            return res;
        }
    }
}
