import { EducationLevel } from "./education-level";

export interface Exam {
  id: string;
  phase: string;
  position: string;
  area: string;
  educationLevel: EducationLevel;
  examBookletUrl: string;
  answerKeyUrl: string;
  answerKeyItems: AnswerKeyItem[];
}

export interface AnswerKeyItem {
  // propriedades da classe AnswerKeyItem
}
