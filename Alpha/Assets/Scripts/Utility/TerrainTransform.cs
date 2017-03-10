﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utility.TerrainAlgorithm
{
    public abstract class TerrainTransform
    {
        /* 
         * Classe base para algoritmos de transformação do terreno
         * 
         * Algoritmos iniciais para experimentação:
         *  Suavização simples (média)
         *  Suavização com depósito de material (média com redistribuição)
         *  Escavação unilateral (média pra baixo de um lado da matriz de transformação)
        */

        public abstract bool IsActive();

        // Deve ser implementado com o algoritmo de transformação
        public abstract void ApplyTransform(ref float[,] heights);
    }
}