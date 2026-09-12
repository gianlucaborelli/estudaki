export interface Exam {
  id: string;
  phase: string;
  position: string;
  area: string;
  educationLevel: string;
  examBookletUrl: string;
  answerKeyUrl: string;
  answerKeyItems: AnswerKeyItem[];
}

export interface AnswerKeyItem {
  // propriedades da classe AnswerKeyItem
}
