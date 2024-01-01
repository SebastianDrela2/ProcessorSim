﻿namespace ProcessorSim.Instructions
{
    internal class InstructionsResolver
    {
        private readonly Processor _processor;
        private readonly InstructionRetriever _actionRetriever;

        public InstructionsResolver(Processor processor, InstructionRetriever actionRetriever)
        {
            _processor = processor;
            _actionRetriever = actionRetriever;

        }
        public Action ResolveInstruction(string line)
        {
            var instruction = ParseInstruction(line);
            var operation = instruction.Operation;

            var value2 = ParseParameterValue(instruction.SecondParameter);
            var value3 = ParseParameterValue(instruction.ThirdParameter);

            if (instruction.FirstParameter.StartsWith("r"))
            {
                var register = ParseRegister(instruction.FirstParameter);
                if (!string.IsNullOrEmpty(instruction.SecondParameter))
                {
                    var secondParameterIsNotAnInt = !int.TryParse(instruction.SecondParameter, out _);

                    if (secondParameterIsNotAnInt && instruction.Operation is "LDR")
                    {
                        return _actionRetriever.LoadDataIntoRegister(register, instruction.SecondParameter);
                    }
                }
                return _actionRetriever.GetArithemticRegisterInstruction(operation, register, value2, value3);
            }

            return _actionRetriever.GetNonRegisterInstruction(operation, instruction.FirstParameter);
        }

        private int ParseParameterValue(string parameter)
        {
            if (parameter.StartsWith('r'))
            {
                return ParseRegister(parameter).Value;
            }

            if (int.TryParse(parameter, out var result))
            {
                return result;
            }

            return 0;
        }

        private Instruction ParseInstruction(string input)
        {
            var parts = input.Split(' ');

            var thirdParameter = string.Empty;

            if (parts.Length > 3)
            {
                thirdParameter = parts[3];
            }

            if (parts.Length > 2)
            {
                return new Instruction
                {
                    Operation = parts[0],
                    FirstParameter = parts[1],
                    SecondParameter = parts[2],
                    ThirdParameter = thirdParameter
                };
            }

            if (parts.Length > 1)
            {
                return new Instruction
                {
                    Operation = parts[0],
                    FirstParameter = parts[1]
                };
            }

            return new Instruction
            {
                Operation = parts[0]
            };
        }

        private Register ParseRegister(string registerLine)
        {
            var subStringedRegisterLine = registerLine.Substring(1);
            var numericPart = int.Parse(subStringedRegisterLine);
            return _processor.Registers[numericPart];

        }
    }
}
